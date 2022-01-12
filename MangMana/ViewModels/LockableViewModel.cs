using MangMana.Helpers;
using System.Windows;
using System.Windows.Input;

namespace MangMana.ViewModels
{
    internal class LockableViewModel : BaseViewModel
    {
        public string LockIcon { get; set; } = "../Icons/lock.png";

        public bool ReadOnlyMode
        {
            get => _readOnlyMode;
            set
            {
                _readOnlyMode = value;
                EditMode = !value;
                LockIcon = value ? "../Icons/lock.png" : "../Icons/unlock.png";
            }
        }

        public bool EditMode { get; private set; }
        public Visibility EditModeVisibility => _readOnlyMode ? Visibility.Collapsed : Visibility.Visible;

        public ICommand ToggleReadOnlyModeCommand { get; }

        private bool _readOnlyMode = true;

        public LockableViewModel()
        {
            ToggleReadOnlyModeCommand = new SimpleCommand(ToggleReadOnlyMode);
        }

        private void ToggleReadOnlyMode()
        {
            ReadOnlyMode = !ReadOnlyMode;
        }
    }
}