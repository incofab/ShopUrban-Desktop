using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShopUrban.Commands
{
    class GenericCommand : ICommand
    {
        private Action<object> executeMethod;
        private Func<object, bool> canExecuteMethod;

        public GenericCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            this.canExecuteMethod = canExecuteMethod;
            this.executeMethod = executeMethod;
        }

        event EventHandler ICommand.CanExecuteChanged
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

        //public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod == null) return true;

            return canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }
    }
}
