using System;
using System.Windows.Input;

namespace CanExecute
{
    public class MyRelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public MyRelayCommand(Action<object> execute) : this(execute, CanAlwaysExecute)
        { }

        public MyRelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            // Lamda expression to execute each respectively
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object cmdParm)
        { return _canExecute(cmdParm); }

        public static bool CanAlwaysExecute(object cmdParm)
        { return true; }

        public void Execute(object cmdParm)
        {
            if (!_doingWithCallback)
                _execute(cmdParm);
            else
                Execute2(cmdParm);
        }

        // The CanExecuteChanged event handler is required from the ICommand interface
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        private bool _isMyTaskRunning = false;
        private bool _doingWithCallback;
        private readonly Action<object, Action> _executeWithCallback;

        public MyRelayCommand(Action<object, Action> executeWithCallback) : this(executeWithCallback, CanAlwaysExecute)
        { }

        public MyRelayCommand(Action<object, Action> executeWithCallback, Func<object, bool> canExecute)
        {
            // new flag, so when the default "Execute" method is called, it can then redirect to
            // calling the Execute2() method that checks to prevent the double-click and then
            // calls your function with the additional parameter of the action method to call upon completion.
            _doingWithCallback = true;
            _executeWithCallback = executeWithCallback;
            _canExecute = canExecute;
        }

        public void Execute2(object cmdParm)
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

            // NOW, call your execute function that accepts TWO parameters.
            // the first is whatever parameter MAY come from the button click itself.
            // the SECOND parameter will be to accept MY ACTION HERE to reset the flag when finished
            System.Threading.Tasks.Task.Run(() => _executeWithCallback(cmdParm, ButtonTaskIsComplete));
        }

        public void ButtonTaskIsComplete()
        {
            _isMyTaskRunning = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { RaiseCanExecuteChanged(); });
        }
    }
}
