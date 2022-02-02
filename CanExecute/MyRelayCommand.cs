using System;
using System.Windows.Input;

namespace CanExecute
{
    public class MyRelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public MyRelayCommand(Action<object> execute) : this(execute, CanAlwaysExecute)
        {
        }

        public MyRelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            // Lamda expression to execute each respectively
            _execute = execute;
            _canExecute = canExecute;
        }

        // The CanExecuteChanged event handler is required from the ICommand interface
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public bool CanExecute(object cmdParm)
        {
            return !_isMyTaskRunning && _canExecute(cmdParm);
        }

        public static bool CanAlwaysExecute(object cmdParm)
        {
            return true;
        }

        public void Execute(object cmdParm)
        {
            // prevent double action if running vs not
            if (_isMyTaskRunning)
                return;

            // flag it to prevent double action
            _isMyTaskRunning = true;

            // trigger raising the CanExecute changed which will update the user interface
            RaiseCanExecuteChanged();

            // turn off when done, but if being done from a "task()" process, 
            // you probably want to have a return function be called when the 
            // TASK is finished to re-enable the button... maybe like
            System.Threading.Tasks.Task.Run(() => _execute)
                .ContinueWith(task => { ButtonTaskIsComplete(); });
        }

        private bool _isMyTaskRunning = false;

        public void ButtonTaskIsComplete()
        {
            _isMyTaskRunning = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { RaiseCanExecuteChanged(); });
        }
    }
}
