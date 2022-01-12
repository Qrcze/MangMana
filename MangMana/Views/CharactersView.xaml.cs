using MangMana.Models;
using MangMana.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MangMana.Views
{
    /// <summary>
    /// Interaction logic for CharactersView.xaml
    /// </summary>
    public partial class CharactersView : UserControl
    {
        private CharactersViewModel _vm;

        public CharactersView()
        {
            InitializeComponent();
            _vm = DataContext as CharactersViewModel;
            _vm.ScrollDown = () => charactersListBox.ScrollIntoView(_vm.Character);
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
            Character draggedItem = e.Data.GetData(typeof(Character)) as Character;
            if (draggedItem == null) return;

            Character target = (sender as ListBoxItem).DataContext as Character;

            int draggedItemIndex = _vm.Book.Characters.IndexOf(draggedItem);
            int targetIndex = _vm.Book.Characters.IndexOf(target);

            if (draggedItemIndex != targetIndex)
                _vm.Book.Characters.Move(draggedItemIndex, targetIndex);

            charactersListBox.SelectedItem = draggedItem;
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.FilterCharacters(filterBox.Text);
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            filterBox.Text = "";
        }

        private void ImageDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] images = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var imgPath in images)
                {
                    if (Global.ImageExtensions.Contains(Path.GetExtension(imgPath)))
                        _vm.AddImage(imgPath);
                }
            }
        }
    }
}