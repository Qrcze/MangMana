using MangMana.Helpers;
using MangMana.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangMana.ViewModels
{
    internal class ExtrasViewModel : LockableViewModel
    {
        public Book Book { get; set; }
        public BitmapImage CurrentImage { get; set; }

        private int _imageNumber;

        public int ImageNumber
        {
            get => _imageNumber;
            set
            {
                _imageNumber = value;
                UpdateImageControls();
            }
        }

        public int ImagesCount { get; set; }
        public int RelativeImageNumber { get; set; }

        public bool NotFirstImage { get; set; }
        public bool NotLastImage { get; set; }

        public ICommand AddImageCommand { get; }
        public ICommand AddImageFromClipboardCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }
        public ICommand OpenImageCommand { get; }
        public ICommand ShiftImageBackCommand { get; }
        public ICommand ShiftImageFwdCommand { get; }

        public ExtrasViewModel()
        {
            BookSelectionChanged += ExtrasViewModel_BookSelectionChanged;

            AddImageCommand = new SimpleCommand(AddImage);
            AddImageFromClipboardCommand = new SimpleCommand(AddImageFromClipboard);
            RemoveImageCommand = new SimpleCommand(RemoveImage);
            NextImageCommand = new SimpleCommand(NextImage);
            PrevImageCommand = new SimpleCommand(PrevImage);
            OpenImageCommand = new SimpleCommand(OpenImage);
            ShiftImageBackCommand = new SimpleCommand(ShiftImageBack);
            ShiftImageFwdCommand = new SimpleCommand(ShiftImageFwd);
        }

        private void ExtrasViewModel_BookSelectionChanged()
        {
            Book = CurrentBook;

            if (Book == null)
                return;

            CurrentImage = Book.GetExtraImage(0);

            ImageNumber = 0;
            UpdateImageControls();
        }

        private void UpdateImageControls()
        {
            NotFirstImage = ImageNumber > 0;
            NotLastImage = ImageNumber < Book.ExtraImages.Count - 1;

            ImagesCount = Book.ExtraImages.Count;

            RelativeImageNumber = ImagesCount > 0 ? ImageNumber + 1 : 0;
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
                Book.AddExtraImage(img);
                ImageNumber = Book.ExtraImages.Count - 1;
                CurrentImage = Book.GetExtraImage(ImageNumber);
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
            Book.AddExtraImage(path);
            ImageNumber = Book.ExtraImages.Count - 1;
            CurrentImage = Book.GetExtraImage(ImageNumber);
        }

        private void RemoveImage()
        {
            if (CurrentImage != null)
            {
                Book.RemoveExtraImage(ImageNumber);
                if (ImageNumber < Book.ExtraImages.Count)
                {
                    CurrentImage = Book.GetExtraImage(ImageNumber);
                    UpdateImageControls();
                }
                else if (Book.ExtraImages.Count > 0)
                {
                    ImageNumber = Book.ExtraImages.Count - 1;
                    CurrentImage = Book.GetExtraImage(ImageNumber);
                }
                else
                {
                    CurrentImage = null;
                    UpdateImageControls();
                }
            }
        }

        private void NextImage()
        {
            ImageNumber++;
            CurrentImage = Book.GetExtraImage(ImageNumber);
        }

        private void PrevImage()
        {
            ImageNumber--;
            CurrentImage = Book.GetExtraImage(ImageNumber);
        }

        private void OpenImage()
        {
            string argument = $"/select, \"{Path.GetFullPath(Book.GetExtraImagePath(ImageNumber))}\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void ShiftImageBack()
        {
            var item = Book.ExtraImages[ImageNumber];
            Book.ExtraImages.Move(ImageNumber, ImageNumber - 1);
            ImageNumber--;
        }

        private void ShiftImageFwd()
        {
            var item = Book.ExtraImages[ImageNumber];
            Book.ExtraImages.Move(ImageNumber, ImageNumber + 1);
            ImageNumber++;
        }
    }
}