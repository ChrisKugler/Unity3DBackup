using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Unity3DPartialBackup
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool lastCanExecuteEval = true;
        private Func<object, bool> canExecute;
        private Action<object> execute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        { }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            bool val = false;
            if (this.canExecute != null)
                val = canExecute(parameter);
            else val = true;

            if (val != this.lastCanExecuteEval)
            {
                this.lastCanExecuteEval = val;
                this.OnCanExecuteChanged();
            }
            return val;
        }

        public void Execute(object parameter)
        {
            if (this.execute != null)
                this.execute(parameter);
        }

        private void OnCanExecuteChanged()
        {
            EventHandler temp = this.CanExecuteChanged;
            if (temp != null)
                temp(this, null);
        }
    }   
}
