using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Services;

namespace Wpf.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        private readonly AuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel(AuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ShowLoginView();
        }

        private void ShowLoginView()
        {
            
            var loginVm = _serviceProvider.GetRequiredService<LoginViewModel>();
            
            loginVm.LoginSucceeded -= OnLoginSucceeded; 
            loginVm.LoginSucceeded += OnLoginSucceeded;

            CurrentView = loginVm;
        }

        private void OnLoginSucceeded()
        {
            
            var mainVm = _serviceProvider.GetRequiredService<MainViewModel>();
            
            mainVm.LogoutRequested -= OnLogout; 
            mainVm.LogoutRequested += OnLogout;

            CurrentView = mainVm;
        }

        private void OnLogout()
        {
            _authService.Logout();
            ShowLoginView();
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}