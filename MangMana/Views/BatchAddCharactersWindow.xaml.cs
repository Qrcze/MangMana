using MangMana.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace MangMana.Views
{
    /// <summary>
    /// Interaction logic for BatchAddCharactersWindow.xaml
    /// </summary>
    public partial class BatchAddCharactersWindow : Window
    {
        private BatchAddCharactersWindowViewModel _vm;

        public BatchAddCharactersWindow(List<Models.Character> charactersList)
        {
            InitializeComponent();
            DataContext = _vm = new BatchAddCharactersWindowViewModel(charactersList);
            _vm.CloseWindow = Close;
        }
    }
}