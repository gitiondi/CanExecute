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
        public AsyncRelayCommand BtnCmd { get; }
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
            BtnCmd = new AsyncRelayCommand(DoSomething, CanDoSomething);
            CheckBoxCmd = new RelayCommand<bool>(Checked);
            IsCheckBoxEnabled = true;
            Title = $"Active: {IsCheckBoxEnabled}";
        }

        private void Checked(bool checkedState)
        {
            IsCheckBoxEnabled = checkedState;
            Title = $"Active: {IsCheckBoxEnabled}";
        }

        private bool CanDoSomething()
        {
            return IsCheckBoxEnabled;
        }

        private async Task DoSomething()
        {
            IsCheckBoxEnabled = false;
            await Task.Delay(1000);
            IsCheckBoxEnabled = true;
        }
    }
}
