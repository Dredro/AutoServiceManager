using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection; 

using Wpf.Core; 
using Wpf.Models; 
using Wpf.Services; 
using Wpf.ViewModel.Manager;
using Wpf.ViewModel.StorageManager;
using Wpf.ViewModel.Worker;
using Wpf.ViewModels; 

namespace Wpf.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? LogoutRequested;

        private readonly IServiceProvider _serviceProvider;
        private readonly AuthService _authService;

        
        private ClientsListViewModel? _clientsListViewModel;
        private CreateClientFormViewModel? _createClientFormViewModel;
        private VehiclesListViewModel? _vehiclesListViewModel; 
        private CreateCarFormViewModel? _createCarFormViewModel; 

        public ICommand LogoutCommand { get; }
        public ICommand SwitchViewCommand { get; }

        private object _currentContent = String.Empty;

        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                if (_currentContent != value)
                {
                    
                    if (_currentContent is ClientsListViewModel oldClientsVm)
                    {
                        oldClientsVm.RequestCreateClientView -= OnRequestCreateClientView;
                        oldClientsVm.RequestShowClientView -= OnRequestShowClientView;
                        oldClientsVm.RequestEditClientView -= OnRequestEditClientView;
                    }
                    if (_currentContent is CreateClientFormViewModel oldCreateClientVm)
                    {
                        oldCreateClientVm.ClientCreated -= OnClientCreated;
                    }
                    
                    if (_currentContent is VehiclesListViewModel oldVehiclesVm)
                    {
                        oldVehiclesVm.RequestCreateVehicleView -= OnRequestCreateVehicleView;
                    }
                    
                    if (_currentContent is CreateCarFormViewModel oldCreateCarVm)
                    {
                        oldCreateCarVm.CarCreated -= OnCarCreated;
                    }

                    _currentContent = value;
                    OnPropertyChanged(nameof(CurrentContent));
                }
            }
        }

        private Role? _currentRole;
        public Role? CurrentRole
        {
            get => _currentRole;
            private set
            {
                if (_currentRole != value)
                {
                    _currentRole = value;
                    OnPropertyChanged(nameof(CurrentRole));
                }
            }
        }

        public MainViewModel(IServiceProvider serviceProvider, AuthService authService)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            LogoutCommand = new RelayCommand(() => LogoutRequested?.Invoke());
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);

            CurrentRole = AuthService.CurrentRole;
            if (CurrentRole == null)
            {
                LogoutRequested?.Invoke();
            }

            OnSwitchView("Dashboard");
        }

        public void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    if (CurrentRole == Role.Mechanic)
                    {
                        CurrentContent = _serviceProvider.GetRequiredService<WorkerDashboardViewModel>();
                    }
                    else if (CurrentRole == Role.StorageManager)
                    {
                        CurrentContent = _serviceProvider.GetRequiredService<StorageManagerDashboardViewModel>();
                    }
                    else if (CurrentRole == Role.Manager)
                    {
                        CurrentContent = _serviceProvider.GetRequiredService<ManagerDashboardViewModel>();
                    }
                    else
                    {
                        CurrentContent = "Brak dostępu do pulpitu dla Twojej roli.";
                    }
                    break;
                case "Orders":
                    CurrentContent = _serviceProvider.GetRequiredService<OrdersViewModel>();
                    break;
                case "Parts":
                    CurrentContent = _serviceProvider.GetRequiredService<OrderFormViewModel>();
                    break;
                case "Customers":
                    _clientsListViewModel = _serviceProvider.GetRequiredService<ClientsListViewModel>();
                    _clientsListViewModel.RequestCreateClientView -= OnRequestCreateClientView;
                    _clientsListViewModel.RequestCreateClientView += OnRequestCreateClientView;
                    _clientsListViewModel.RequestShowClientView -= OnRequestShowClientView;
                    _clientsListViewModel.RequestShowClientView += OnRequestShowClientView;
                    _clientsListViewModel.RequestEditClientView -= OnRequestEditClientView;
                    _clientsListViewModel.RequestEditClientView += OnRequestEditClientView;
                    CurrentContent = _clientsListViewModel;
                    break;
                case "Cars":
                    _vehiclesListViewModel = _serviceProvider.GetRequiredService<VehiclesListViewModel>(); 
                    
                    _vehiclesListViewModel.RequestCreateVehicleView -= OnRequestCreateVehicleView; 
                    _vehiclesListViewModel.RequestCreateVehicleView += OnRequestCreateVehicleView;
                    CurrentContent = _vehiclesListViewModel;
                    break;
                case "CreateCar": 
                    _createCarFormViewModel = _serviceProvider.GetRequiredService<CreateCarFormViewModel>();
                    _createCarFormViewModel.CarCreated -= OnCarCreated; 
                    _createCarFormViewModel.CarCreated += OnCarCreated; 
                    CurrentContent = _createCarFormViewModel;
                    break;
                case "CreateClient":
                    _createClientFormViewModel = _serviceProvider.GetRequiredService<CreateClientFormViewModel>();
                    _createClientFormViewModel.ClientCreated -= OnClientCreated;
                    _createClientFormViewModel.ClientCreated += OnClientCreated;
                    CurrentContent = _createClientFormViewModel;
                    break;
                case "ShowClient":
                    MessageBox.Show("Nawigacja do szczegółów klienta niezaimplementowana.", "Info");
                    break;
                case "EditClient":
                    MessageBox.Show("Nawigacja do edycji klienta niezaimplementowana.", "Info");
                    break;
                default:
                    CurrentContent = "Nieznany widok.";
                    break;
            }
        }

        
        private void OnRequestCreateClientView()
        {
            OnSwitchView("CreateClient");
        }

        private void OnRequestCreateVehicleView() 
        {
            OnSwitchView("CreateCar"); 
        }

        private async void OnCarCreated() 
        {
            MessageBox.Show("Nowy samochód został pomyślnie dodany! Odświeżam listę pojazdów.", "Sukces");
            OnSwitchView("Cars"); 
        }

        private void OnRequestShowClientView(Guid clientId)
        {
            MessageBox.Show($"Wyświetl szczegóły klienta o ID: {clientId}", "Info");
        }

        private void OnRequestEditClientView(Guid clientId)
        {
            MessageBox.Show($"Edytuj klienta o ID: {clientId}", "Info");
        }

        private async void OnClientCreated()
        {
            MessageBox.Show("Nowy klient został pomyślnie dodany! Odświeżam listę klientów.", "Sukces");
            OnSwitchView("Customers"); 
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}