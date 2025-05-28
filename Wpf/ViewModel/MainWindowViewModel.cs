using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wpf.Views;

namespace Wpf.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel()
        {
            var loginVm = new LoginViewModel();
            loginVm.LoginSucceeded += OnLoginSucceeded;

            CurrentView = new LoginPage { DataContext = loginVm };
        }

        private void OnLoginSucceeded()
        {
            var mainVm = new MainViewModel();
            mainVm.LogoutRequested += OnLogout;

            CurrentView = new MainView { DataContext = mainVm };
        }

        private void OnLogout()
        {
            var loginVm = new LoginViewModel();
            loginVm.LoginSucceeded += OnLoginSucceeded;

            CurrentView = new LoginPage { DataContext = loginVm };
        }

        //Nie wiem czy to jest potrzebne, ale zostawiam
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
