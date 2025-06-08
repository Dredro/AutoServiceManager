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

        public Role RoleLocal
        {
            get { return role; }
            set { role = value; }
        }

        public MainViewModel()
        {
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);
            WorkerDashboardViewModel = new WorkerDashboardViewModel();
            OrderFormViewModel = new OrderFormViewModel();
            OrdersViewModel = new OrdersViewModel();
            OnSwitchView("Dashboard");
        }

        public ICommand SwitchViewCommand { get; }
        public WorkerDashboardViewModel WorkerDashboardViewModel { get; set; }
        public OrderFormViewModel OrderFormViewModel { get; set; }
        public OrdersViewModel OrdersViewModel { get; set; }

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);
            WorkerDashboardViewModel = new WorkerDashboardViewModel();
            OrderFormViewModel = new OrderFormViewModel();

            OnSwitchView("Dashboard");
        }

        public Visibility IsAdminVisible => RoleLocal == Role.Admin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsManagerVisible => RoleLocal == Role.Manager ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsMechanicVisible => RoleLocal == Role.Mechanic ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsStorageManagerVisible => RoleLocal == Role.StorageManager ? Visibility.Visible : Visibility.Collapsed;

        public void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    switch (RoleLocal)
                    {
                        case Role.Mechanic:
                            CurrentContent = WorkerDashboardViewModel;
                            break;
                        case Role.StorageManager:
                            CurrentContent = WorkerDashboardViewModel;
                            break;
                        case Role.Manager:
                            CurrentContent = WorkerDashboardViewModel;
                            break;
                        case Role.Admin:
                            CurrentContent = WorkerDashboardViewModel;
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
