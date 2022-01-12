using MangMana.Helpers;
using MangMana.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MangMana.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public Visibility BookOpenedVisibility { get; set; } = Visibility.Hidden;

        public ICommand AddBookCommand { get; }
        public ICommand DeleteBookCommand { get; }

        public ICollectionView Library
        {
            get
            {
                var source = CollectionViewSource.GetDefaultView(Books);
                source.Filter = (p) => (p as Book).Name.Contains(_filter, System.StringComparison.InvariantCultureIgnoreCase);
                return source;
            }
        }

        private string _filter = "";

        public MainWindowViewModel()
        {
            AddBookCommand = new SimpleCommand(AddBook);
            DeleteBookCommand = new SimpleCommand(DeleteBook);

            BookSelectionChanged += () => BookOpenedVisibility = CurrentBook == null ? Visibility.Hidden : Visibility.Visible;
        }

        private void AddBook()
        {
            var book = Book.Create();

            Books.Add(book);
            CurrentBook = book;
        }

        private void DeleteBook()
        {
            var result = MessageBox.Show($"Are you sure you want to remove book: {CurrentBook.Name}?\nThis decision cannot be undone.", $"Remove book: {CurrentBook.Name}?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Books.Remove(CurrentBook);
        }

        public void FilterBooks(string text)
        {
            _filter = text;
            Library.Refresh();
        }
    }
}