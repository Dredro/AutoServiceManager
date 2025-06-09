using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models;
using Wpf.Services;
using Wpf.ViewModel.Manager;
using Wpf.ViewModel.StorageManager;
using Wpf.ViewModel.Worker;
using Wpf.Views;
using Wpf.Views.Worker;

namespace Wpf.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? LogoutRequested;
        IServiceProvider _serviceProvider;

        public ICommand LogoutCommand { get; }

        private object _currentContent = String.Empty;

        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentContent)));
            }
        }
        private Role role = Role.Mechanic;

        public Role? CurrentRole { get; set; }


        public ICommand SwitchViewCommand { get; }
        public WorkerDashboardViewModel WorkerDashboardViewModel { get; set; }
        public StorageManagerDashboardViewModel StorageManagerDashboardViewModel { get; set; } = new();
        public ManagerDashboardViewModel ManagerDashboardViewModel { get; set; } = new();
        public OrderFormViewModel OrderFormViewModel { get; set; }
        public OrdersViewModel OrdersViewModel { get; set; }

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);
            WorkerDashboardViewModel = new WorkerDashboardViewModel();
            CurrentRole = AuthService.CurrentRole;
            if (CurrentRole == null)
            {
                LogoutRequested?.Invoke();
            }
            OrderFormViewModel = new OrderFormViewModel();

            OnSwitchView("Dashboard");
        }

        public Visibility IsAdminVisible => CurrentRole == Role.Admin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsManagerVisible =>CurrentRole == Role.Manager ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsMechanicVisible => CurrentRole == Role.Mechanic ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsStorageManagerVisible => CurrentRole == Role.StorageManager ? Visibility.Visible : Visibility.Collapsed;

        public void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    switch (CurrentRole)
                    {
                        case Role.Mechanic:
                            CurrentContent = WorkerDashboardViewModel;
                            break;
                        case Role.StorageManager:
                            CurrentContent = StorageManagerDashboardViewModel;
                            break;
                        case Role.Manager:
                            CurrentContent = ManagerDashboardViewModel;
                            break;
                        case Role.Admin:
                            CurrentContent = ManagerDashboardViewModel;
                            break;
                        default:
                            break;
                    }
                    break;
                case "Orders":
                    CurrentContent = OrdersViewModel;
                    break;
                case "Parts":
                    CurrentContent = OrderFormViewModel;
                    break;
                case "Customers":
                    CurrentContent = new ClientsListView { DataContext = this };
                    break;
                case "Cars":
                    CurrentContent = new VehiclesListView { DataContext = this };
                    break;
            }
        }

    }
}
