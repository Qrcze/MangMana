using MangMana.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MangMana.Models
{
    internal class Book : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string UID { get; set; }

        public string Name { get; set; } = "Book";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Notes1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Notes2 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ThumbnailName { get; set; }

        public ObservableCollection<Character> Characters { get; set; } = new();
        public ObservableCollection<string> ExtraImages { get; set; } = new();

        [JsonIgnore] private bool Ready { get; set; }

        public string GetDirectoryPath() => $"{Global.DataCatalog}/thumbnails/{UID}";

        public string GetThumbnailPath() => $"{GetDirectoryPath()}/{ThumbnailName}.thumb";

        public void OnDeserialize()
        {
            foreach (var character in Characters)
                LoadCharacter(character);

            StartWatchingProperties();

            Ready = true;
        }

        private void BookChanged(object sender, EventArgs e)
        {
            if (!Ready)
                return;
            Debug.Print($"Book changed");

            string dataPath = $"{GetDirectoryPath()}/{UID}.json";

            try
            {
                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(dataPath, json);
            }
            catch (Exception exc)
            {
                string message;

                if (exc is JsonException)
                    message = $"Error while saving book data json file to {dataPath};\n{exc}";
                else if (exc is FileNotFoundException || exc is DirectoryNotFoundException)
                    message = $"Couldn't find the file in {dataPath};\n{exc}";
                else
                    message = $"Unhandled exception while saving book data to {dataPath}:\n{exc}";

                MessageBox.Show(message, "Error while saving book data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartWatchingProperties()
        {
            PropertyChanged += BookChanged;
            Characters.CollectionChanged += BookChanged;
            ExtraImages.CollectionChanged += BookChanged;
        }

        /// <summary>
        /// Generates UID for the book and creates a folder
        /// </summary>
        public static Book Create()
        {
            var book = new Book();
            book.StartWatchingProperties();

            // Get an available UID for a folder
            book.UID = SimpleUID.Generate((s) => !File.Exists($"{Global.DataCatalog}/thumbnails/{s}"));

            // Create the folder
            Directory.CreateDirectory(book.GetDirectoryPath());

            book.Ready = true;

            return book;
        }

        public void AddCharacter(Character character)
        {
            LoadCharacter(character);
            Characters.Add(character);
        }

        /// <summary>
        /// Sets BookUID for character and starts observing it's changes
        /// </summary>
        private void LoadCharacter(Character character)
        {
            character.BookUID = UID;

            character.PropertyChanged += BookChanged;
            character.ImageUIDs.CollectionChanged += BookChanged;
        }

        public void RemoveCharacter(Character character)
        {
            Characters.Remove(character);
        }

        public void SetThumbnail(string imagePath)
        {
            var img = ImageHelpers.CreateBitmapImage(imagePath, 400);

            ThumbnailName = Path.GetRandomFileName().Replace(".", "");

            ImageHelpers.SaveImage(img, GetThumbnailPath());
        }

        public void DeleteThumbnail()
        {
            File.Delete(GetThumbnailPath());
            ThumbnailName = null;
        }

        public BitmapImage GetThumbnail()
        {
            if (!File.Exists(GetThumbnailPath()))
                return null;
            return ImageHelpers.CreateBitmapImage(GetThumbnailPath(), 400);
        }

        public void AddExtraImage(string path)
        {
            var img = ImageHelpers.CreateBitmapImage(path);

            AddExtraImage(img);
        }

        public void AddExtraImage(BitmapSource img)
        {
            string uid = SimpleUID.Generate(uid => !File.Exists(GetDirectoryPath()));

            ImageHelpers.SaveImage(img, $"{GetDirectoryPath()}/{uid}.jpg");

            ExtraImages.Add(uid);
        }

        public BitmapImage GetExtraImage(int index)
        {
            if (index >= ExtraImages.Count)
                return null;

            return ImageHelpers.CreateBitmapImage(GetExtraImagePath(index));
        }

        public void RemoveExtraImage(int index)
        {
            File.Delete(ExtraImages[index]);
            ExtraImages.RemoveAt(index);
        }

        public string GetExtraImagePath(int index)
        {
            return $"{GetDirectoryPath()}/{ExtraImages[index]}.jpg";
        }
    }
}