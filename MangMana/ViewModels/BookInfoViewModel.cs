using MangMana.Helpers;
using MangMana.Models;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MangMana.ViewModels
{
    internal class BookInfoViewModel : LockableViewModel
    {
        public Book Book { get; set; }
        public BitmapImage Image { get; set; }

        public ICommand ChangeThumbnailCommand { get; }
        public ICommand RemoveThumbnailCommand { get; }

        public BookInfoViewModel()
        {
            BookSelectionChanged += BookInfoViewModel_BookChanged;

            ChangeThumbnailCommand = new SimpleCommand(OpenChangeThumbnailWindow);
            RemoveThumbnailCommand = new SimpleCommand(DeleteThumbnail);
        }

        private void BookInfoViewModel_BookChanged()
        {
            Book = CurrentBook;
            Image = Book?.GetThumbnail();
            ReadOnlyMode = true;
        }

        private void OpenChangeThumbnailWindow()
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "Image File|*.jpg;*.png;*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                ChangeThumbnail(dialog.FileName);
            }
        }

        public void ChangeThumbnail(string path)
        {
            Book.SetThumbnail(path);
            Image = Book.GetThumbnail();
        }

        private void DeleteThumbnail()
        {
            Book.DeleteThumbnail();
            Image = null;
        }
    }
}