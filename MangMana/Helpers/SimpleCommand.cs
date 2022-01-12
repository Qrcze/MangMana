using System;
using System.Windows.Input;

namespace MangMana.Helpers
{
    internal class SimpleCommand : ICommand
    {
        private readonly Action _action;

        public SimpleCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }

    internal class SimpleCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public SimpleCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}