using MangMana.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MangMana.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public static ObservableCollection<Book> Books { get; private set; } = new();

        private static Book _book;

        public static Book CurrentBook
        {
            get => _book;
            set
            {
                _book = value;
                BookSelectionChanged.Invoke();
            }
        }

        public static event Action BookSelectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}