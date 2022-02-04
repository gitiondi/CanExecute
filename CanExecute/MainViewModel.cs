using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace CanExecute
{
    class MainViewModel : ObservableObject
    {
        private bool _isCheckBoxEnabled;
        private string _title;
        private MyRelayCommand _btnCmd;

        //public MyRelayCommand BtnCmd { get; }
        public MyRelayCommand BtnCmd => _btnCmd ??= new MyRelayCommand(DoSomething, CanDoSomething);


        public ICommand CheckBoxCmd { get; }

        public bool IsCheckBoxEnabled
        {
            get => _isCheckBoxEnabled;
            set => SetProperty(ref _isCheckBoxEnabled, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainViewModel()
        {
            //BtnCmd = new MyRelayCommand(DoSomething, executeCallback, CanDoSomething);
            CheckBoxCmd = new RelayCommand<bool>(Checked);
            IsCheckBoxEnabled = true;
            Title = $"Active: {IsCheckBoxEnabled}";
        }

        private bool executeCallback(object obj)
        {
            return true;
        }

        private void Checked(bool checkedState)
        {
            IsCheckBoxEnabled = checkedState;
            Title = $"Active: {IsCheckBoxEnabled}";
        }

        private bool CanDoSomething(object obj)
        {
            Debug.WriteLine($"--- {IsCheckBoxEnabled}");
            return IsCheckBoxEnabled && !BtnCmd.IsMyTaskRunning;
        }

        private async void DoSomething(object cmdParm, System.Action doThisWhenFinished)
        {
            IsCheckBoxEnabled = false;
            await Task.Delay(4000);
            IsCheckBoxEnabled = true;
            doThisWhenFinished();
        }
    }
}
