using MangMana.Models;
using MangMana.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace MangMana
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Timer _cleanupTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Make sure the dir exists
            Directory.CreateDirectory(Global.ThumbnailsCatalog);

            LoadLibrary();

            BaseViewModel.Books.CollectionChanged += (s, e) => SaveBookMetadata();

            // Run a files cleanup after 5 seconds
            _cleanupTimer = new Timer(5_000);
            _cleanupTimer.Elapsed += (s, e) => { RemoveUnusedFiles(); _cleanupTimer.Dispose(); };
            _cleanupTimer.Start();
        }

        private static void LoadLibrary()
        {
            if (File.Exists(Global.MetadataPath))
            {
                List<BookMetadata> metadata = null;

                // Load the metadata file
                try
                {
                    string dataJson = File.ReadAllText(Global.MetadataPath);
                    metadata = JsonSerializer.Deserialize<List<BookMetadata>>(dataJson);
                }
                catch (Exception e)
                {
                    CreateBackup();

                    string message;

                    if (e is JsonException)
                        message = $"Failed to deserialize metadata file!";
                    else
                        message = $"Unhandled exception while trying to load books metadata!\n{e}";

                    message += "\n\nA full data folder backup has been created.";

                    MessageBox.Show(message, "Data Corruption", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                bool createdBackup = false;
                List<Book> books = new();

                // Load the books
                foreach (var meta in metadata)
                {
                    string bookPath = $"{Global.ThumbnailsCatalog}/{meta.UID}/{meta.UID}.json";

                    try
                    {
                        string bookJson = File.ReadAllText(bookPath);
                        var book = JsonSerializer.Deserialize<Book>(bookJson);
                        book.OnDeserialize();

                        books.Add(book);
                    }
                    catch (Exception e)
                    {
                        if (!createdBackup)
                        {
                            CreateBackup();
                            createdBackup = true;
                        }

                        string message;

                        if (e is JsonException)
                            message = $"Failed to deserialize book \"{meta.Name}\" (located in: {bookPath})!";
                        else if (e is FileNotFoundException || e is DirectoryNotFoundException)
                            message = $"Couldn't find book data file \"{meta.Name}\" (located in: {bookPath})!";
                        else
                            message = $"Unhandled exception while trying to load book \"{meta.Name}\" (located in: {bookPath})!\n{e}";

                        message += "\n\nA full data folder backup has been created.";

                        MessageBox.Show(message, "Data Corruption", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Add books to the library
                books.ForEach(BaseViewModel.Books.Add);

                // Recreate metadata if loading it had errors
                if (createdBackup)
                    SaveBookMetadata();
            }
            else
            {
                //if book directories are not empty but there's no metadata.json file
                if (Directory.EnumerateDirectories(Global.ThumbnailsCatalog).Any())
                {
                    CreateBackup();

                    string message = $"Book folders exist but a metadata.json file ({Global.MetadataPath}) does not!" +
                            $"\nA full data folder backup has been created.";

                    MessageBox.Show(message, "Data Corruption", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                string json = JsonSerializer.Serialize(Enumerable.Empty<string>());
                File.WriteAllText(Global.MetadataPath, json);
            }
        }

        private static void SaveBookMetadata()
        {
            Debug.Print("Saving book metadata file");

            try
            {
                var metadata = BaseViewModel.Books.Select(x => new BookMetadata(x.Name, x.UID));

                string json = JsonSerializer.Serialize(metadata);
                File.WriteAllText(Global.MetadataPath, json);
            }
            catch (Exception e)
            {
                string message;

                if (e is JsonException)
                    message = $"Error while saving data.json;\n{e}";
                else if (e is FileNotFoundException || e is DirectoryNotFoundException)
                    message = $"Error while opening data.json;\n{e}";
                else
                    message = $"Unhandled exception while saving books metadata:\n{e}";

                MessageBox.Show(message, "Error while saving metadata.json", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void CreateBackup()
        {
            string date = DateTime.Now.ToString().Replace('.', '_').Replace(':', '_').Replace(' ', '_');
            string backupDir = $"data_backup_{date}";

            Directory.CreateDirectory(backupDir);

            //copy data.json
            if (File.Exists(Global.MetadataPath))
                File.Copy(Global.MetadataPath, $"{backupDir}/data.json");

            //copy thumbnails subfolders
            foreach (var dirPath in Directory.GetDirectories(Global.ThumbnailsCatalog))
                Directory.CreateDirectory($"{backupDir}{dirPath.Substring(Global.DataCatalog.Length)}");

            //copy thumbnail files
            foreach (var filePath in Directory.GetFiles(Global.ThumbnailsCatalog, "*.*", SearchOption.AllDirectories))
                File.Copy(filePath, $"{backupDir}{filePath.Substring(Global.DataCatalog.Length)}");
        }

        /// <summary>
        /// Used by App.xaml LockableTextBox style
        /// </summary>
        private void UnlockTextBox(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.DataContext is LockableViewModel view && view.ReadOnlyMode)
            {
                view.ToggleReadOnlyModeCommand.Execute(null);
            }
        }

        private static void RemoveUnusedFiles()
        {
            Debug.Print("Removing unused files...");

            var dirs = Directory.GetDirectories(Global.ThumbnailsCatalog);
            foreach (var dir in dirs)
            {
                var d = dir.Replace('\\', '/');

                var book = BaseViewModel.Books.FirstOrDefault(x => x.GetDirectoryPath() == d);

                //if no book uses that directory - remove it
                if (book == null)
                    Directory.Delete(dir, true);

                //else - check for unused characters thumbnails
                else
                {
                    //get all of the files besides .json file
                    var images = Directory.GetFiles(d).Where(x => !x.EndsWith(".json"));

                    foreach (var image in images)
                    {
                        var i = Path.GetFileNameWithoutExtension(image);

                        bool delete = true;

                        //check if it's the thumbnail
                        if (i == book.ThumbnailName)
                            delete = false;

                        //check if it's one of the extra images
                        else if (book.ExtraImages.FirstOrDefault(x => x == i) != null)
                            delete = false;

                        //check if it's one of the character thumbnails
                        else if (book.Characters.SelectMany(x => x.ImageUIDs).FirstOrDefault(y => y == i) != null)
                            delete = false;

                        if (delete)
                            File.Delete(image);
                    }
                }
            }

            Debug.Print("Done");
        }
    }
}