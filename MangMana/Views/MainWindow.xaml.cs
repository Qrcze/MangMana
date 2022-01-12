using MangMana.Models;
using MangMana.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MangMana.Views
{
    /// <summary>
    /// Interaction logic for LibraryView.xaml
    /// </summary>
    public partial class LibraryView : Window
    {
        private MainWindowViewModel _vm;

        public LibraryView()
        {
            InitializeComponent();
            _vm = DataContext as MainWindowViewModel;
        }

        private void ListBoxItem_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (sender is ListBoxItem draggedItem)
                {
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            Book draggedItem = e.Data.GetData(typeof(Book)) as Book;
            Book target = (sender as ListBoxItem).DataContext as Book;

            if (draggedItem == null)
                return;

            int draggedItemIndex = BaseViewModel.Books.IndexOf(draggedItem);
            int targetIndex = BaseViewModel.Books.IndexOf(target);

            if (draggedItemIndex != targetIndex)
                BaseViewModel.Books.Move(draggedItemIndex, targetIndex);

            booksListBox.SelectedItem = draggedItem;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.FilterBooks(filterBox.Text);
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            filterBox.Text = "";
        }
    }
}