using System;
using System.Windows;
using System.Windows.Input;

namespace SpaceAnalyzer.Commands
{
    public class Command : ICommand
    {
        Action<object> myAction { get; set; }
        Func<object, bool> myFunc { get; set; }

        public Command(Action<object> action, Func<object, bool> func)
        {
            myAction = action;
            myFunc = func;
        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute(object parameter)
        {
            return myFunc.Invoke(parameter);
        }
        public void Execute(object parameter)
        {
            myAction.Invoke(parameter);
        }
    }
}