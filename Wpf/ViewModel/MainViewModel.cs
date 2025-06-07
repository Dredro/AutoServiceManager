using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Role Role
        {
            get { return role; }
            set { role = value; }
        }



        public ICommand SwitchViewCommand { get; }
        public WorkerDashboardViewModel WorkerDashboardViewModel { get; set; }

        public MainViewModel()
        {
            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);
            WorkerDashboardViewModel = new WorkerDashboardViewModel();

            OnSwitchView("Dashboard");
        }

        public void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    switch (Role)
                    {
                        case Role.Mechanic:
                            CurrentContent = WorkerDashboardViewModel;
                            break;
                        case Role.StorageManager:
                            break;
                        case Role.Manager:
                            break;
                        case Role.Admin:
                            break;
                        default:
                            break;
                    }
                    break;
                case "Orders":
                    CurrentContent = new OrdersView { DataContext = this };
                    break;
                case "Parts":
                    CurrentContent = new OrderFormView { DataContext = this };
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
