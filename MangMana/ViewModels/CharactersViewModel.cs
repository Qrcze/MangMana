using MangMana.Helpers;
using MangMana.Models;
using MangMana.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangMana.ViewModels
{
    internal class CharactersViewModel : LockableViewModel
    {
        public Book Book { get; set; }
        public bool IsBookOpened { get; set; }

        public ICollectionView Characters
        {
            get
            {
                if (Book == null)
                    return null;

                var source = CollectionViewSource.GetDefaultView(Book.Characters);
                source.Filter = (p) => (p as Character).Name.Contains(_filter, System.StringComparison.InvariantCultureIgnoreCase);
                return source;
            }
        }

        public BitmapImage CurrentImage { get; set; }

        public Character Character
        {
            get => _character;
            set
            {
                _character = value;

                CurrentImage = value?.GetCurrentImage();

                ImageNumber = value != null ? value.CurrentImage : 0;
                UpdateImageControls();

                IsCharacterSelected = Character != null;
            }
        }

        public bool IsCharacterSelected { get; set; }

        public int ImageNumber
        {
            get => _imageNumber;
            set
            {
                _imageNumber = value;
                UpdateImageControls();
            }
        }

        private void UpdateImageControls()
        {
            NotFirstImage = ImageNumber > 0;
            NotLastImage = Character == null || ImageNumber < Character.ImageUIDs.Count - 1;

            ImagesCount = Character == null ? 0 : Character.ImageUIDs.Count;

            RelativeImageNumber = ImagesCount > 0 ? ImageNumber + 1 : 0;
        }

        public bool NotFirstImage { get; set; }
        public bool NotLastImage { get; set; }
        public int RelativeImageNumber { get; set; }
        public int ImagesCount { get; set; }

        public ICommand PrevImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand AddImageFromClipboardCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand AddCharacterCommand { get; }
        public ICommand BatchAddCharactersCommand { get; }
        public ICommand RemoveCharacterCommand { get; }
        public ICommand ShiftImageBackCommand { get; }
        public ICommand ShiftImageFwdCommand { get; }
        public ICommand OpenImageInExplorerCommand { get; }

        public Action ScrollDown;

        private Character _character;
        private int _imageNumber = 0;
        private string _filter = "";

        public CharactersViewModel()
        {
            BookSelectionChanged += CharactersViewModel_BookChanged;

            PrevImageCommand = new SimpleCommand(PrevImage);
            NextImageCommand = new SimpleCommand(NextImage);
            AddImageCommand = new SimpleCommand(AddImage);
            AddImageFromClipboardCommand = new SimpleCommand(AddImageFromClipboard);
            RemoveImageCommand = new SimpleCommand(RemoveImage);
            AddCharacterCommand = new SimpleCommand(AddCharacter);
            BatchAddCharactersCommand = new SimpleCommand(BatchAddCharacters);
            RemoveCharacterCommand = new SimpleCommand(RemoveCharacter);
            ShiftImageBackCommand = new SimpleCommand(ShiftImageBack);
            ShiftImageFwdCommand = new SimpleCommand(ShiftImageFwd);
            OpenImageInExplorerCommand = new SimpleCommand(OpenImageInExplorer);
        }

        private void CharactersViewModel_BookChanged()
        {
            Book = CurrentBook;
            IsBookOpened = Book != null;
            ReadOnlyMode = true;
        }

        private void PrevImage()
        {
            ImageNumber = --Character.CurrentImage;
            CurrentImage = Character.GetCurrentImage();
        }

        private void NextImage()
        {
            ImageNumber = ++Character.CurrentImage;
            CurrentImage = Character.GetCurrentImage();
        }

        private void AddImage()
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "Image File|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                AddImage(dialog.FileName);
            }
        }

        private void AddImageFromClipboard()
        {
            var img = Clipboard.GetImage();
            if (img != null)
            {
                Character.AddImage(img);
                ImageNumber = Character.CurrentImage = Character.ImageUIDs.Count - 1;
                if (ImageNumber == 0) UpdateImageControls();
                CurrentImage = Character.GetCurrentImage();
            }
            else
            {
                foreach (var file in Clipboard.GetFileDropList())
                {
                    if (Global.ImageExtensions.Contains(Path.GetExtension(file)))
                        AddImage(file);
                }
            }
        }

        public void AddImage(string path)
        {
            Character.AddImage(path);
            ImageNumber = Character.CurrentImage = Character.ImageUIDs.Count - 1;
            if (ImageNumber == 0) UpdateImageControls();
            CurrentImage = Character.GetCurrentImage();
        }

        private void RemoveImage()
        {
            if (CurrentImage != null)
            {
                Character.RemoveImage(ImageNumber);

                if (ImageNumber < Character.ImageUIDs.Count)
                {
                    CurrentImage = Character.GetCurrentImage();
                    UpdateImageControls();
                }
                else if (Character.ImageUIDs.Count > 0)
                {
                    ImageNumber = Character.CurrentImage = Character.ImageUIDs.Count - 1;
                    CurrentImage = Character.GetCurrentImage();
                }
                else
                {
                    CurrentImage = null;
                    UpdateImageControls();
                }
            }
        }

        private void ShiftImageBack()
        {
            var currentImg = Character.ImageUIDs[ImageNumber];
            Character.ImageUIDs.Move(ImageNumber, ImageNumber - 1);

            ImageNumber = --Character.CurrentImage;
        }

        private void ShiftImageFwd()
        {
            var currentImg = Character.ImageUIDs[ImageNumber];
            Character.ImageUIDs.Move(ImageNumber, ImageNumber + 1);

            ImageNumber = ++Character.CurrentImage;
        }

        private void OpenImageInExplorer()
        {
            string argument = $"/select, \"{Path.GetFullPath(Character.GetImagePath(ImageNumber))}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void AddCharacter()
        {
            Character = new Character();
            Book.AddCharacter(Character);
            ScrollDown.Invoke();
        }

        private void BatchAddCharacters()
        {
            List<Character> charactersList = new List<Character>();
            new BatchAddCharactersWindow(charactersList).ShowDialog();

            foreach (var character in charactersList)
            {
                Book.AddCharacter(character);
            }
        }

        private void RemoveCharacter()
        {
            CurrentImage = null;

            var charIndex = Book.Characters.IndexOf(Character);

            Book.RemoveCharacter(Character);

            if (Book.Characters.Count > charIndex)
                Character = Book.Characters[charIndex];
            else if (Book.Characters.Count > 0)
                Character = Book.Characters[^1];
        }

        public void FilterCharacters(string text)
        {
            _filter = text;
            Characters.Refresh();
        }
    }
}