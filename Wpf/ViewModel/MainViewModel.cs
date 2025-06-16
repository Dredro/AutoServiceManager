using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.Extensions.DependencyInjection; 
using Wpf.Core; 
using Wpf.Models;
using Wpf.Models.DTOs;
using Wpf.Services; 
using Wpf.ViewModel.Manager;
using Wpf.ViewModel.Worker;
using Wpf.ViewModels;
using Wpf.ViewModels.StorageManager;

namespace Wpf.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? LogoutRequested;

        private readonly IServiceProvider _serviceProvider;
        private readonly AuthService _authService;

        private StorageManagerDashboardViewModel _storageManagerDashboardViewModel;
        private ClientsListViewModel? _clientsListViewModel;
        private CreateClientFormViewModel? _createClientFormViewModel;
        private VehiclesListViewModel? _vehiclesListViewModel; 
        private CreateCarFormViewModel? _createCarFormViewModel; 
        private ServicesListViewModel? _servicesListViewModel; 
        private CreateServiceFormViewModel? _createServiceFormViewModel; 
        private CreatePartFormViewModel? _createPartFormViewModel;
        private OrdersViewModel? _ordersViewModel;
        private OrderFormViewModel? _orderFormViewModel;
        private EditClientFormViewModel? _editClientFormViewModel;
        private EditServiceFormViewModel? _editServiceFormViewModel;
        private EditVehicleFormViewModel? _editVehicleFormViewModel;
        private WorkerDashboardViewModel? _workerDashboardViewModel;
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
                        oldCreateClientVm.RequestGoBack -= OnEditClientFormGoBack; 
                    }
                    
                    if (_currentContent is VehiclesListViewModel oldVehiclesVm)
                    {
                        oldVehiclesVm.RequestCreateVehicleView -= OnRequestCreateVehicleView;
                        oldVehiclesVm.RequestEditVehicleView -= OnRequestEditVehicleView; // NEW: Unsubscribe
                    }
                    
                    if (_currentContent is CreateCarFormViewModel oldCreateCarVm)
                    {
                        oldCreateCarVm.CarCreated -= OnCarCreated;
                    }

                    if (_currentContent is EditVehicleFormViewModel oldEditVehicleVm) // NEW: Unsubscribe
                    {
                        oldEditVehicleVm.VehicleUpdated -= OnVehicleUpdated;
                        oldEditVehicleVm.RequestGoBack -= OnEditVehicleFormGoBack;
                    }
                    
                    if (_currentContent is CreateServiceFormViewModel oldCreateServiceVm)
                    {
                        oldCreateServiceVm.ServiceCreated -= OnServiceCreated;
                    }

                    if (_currentContent is StorageManagerDashboardViewModel oldStorageManagerDashboardViewModel)
                    {
                        oldStorageManagerDashboardViewModel.AddPartRequested -= OnRequestCreatePartView;
                    }

                    if (_currentContent is CreatePartFormViewModel oldCreatePartFormVm)
                    {
                        _createPartFormViewModel.PartCreated -= OnPartCreated;
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
                    UpdateRoleBasedVisibility(); 
                }
            }
        }

        private bool _isAdminVisible; 
        public bool IsAdminVisible
        {
            get => _isAdminVisible;
            private set
            {
                if (_isAdminVisible != value)
                {
                    _isAdminVisible = value;
                    OnPropertyChanged(nameof(IsAdminVisible));
                }
            }
        }

        private bool _isManagerVisible;
        public bool IsManagerVisible
        {
            get => _isManagerVisible;
            private set
            {
                if (_isManagerVisible != value)
                {
                    _isManagerVisible = value;
                    OnPropertyChanged(nameof(IsManagerVisible));
                }
            }
        }

        private bool _isMechanicVisible;
        public bool IsMechanicVisible
        {
            get => _isMechanicVisible;
            private set
            {
                if (_isMechanicVisible != value)
                {
                    _isMechanicVisible = value;
                    OnPropertyChanged(nameof(IsMechanicVisible));
                }
            }
        }

        private bool _isStorageManagerVisible;
        public bool IsStorageManagerVisible
        {
            get => _isStorageManagerVisible;
            private set
            {
                if (_isStorageManagerVisible != value)
                {
                    _isStorageManagerVisible = value;
                    OnPropertyChanged(nameof(IsStorageManagerVisible));
                }
            }
        }
        
        public MainViewModel() : this(null!, null!) 
        {
            CurrentRole = Role.Manager; 
            OnSwitchView("Dashboard"); 
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
            
            UpdateRoleBasedVisibility(); 

            if(CurrentRole != null)
                OnSwitchView("Dashboard"); 
        }

        private void UpdateRoleBasedVisibility()
        {
            IsAdminVisible = (CurrentRole == Role.Admin); 
            IsManagerVisible = (CurrentRole == Role.Manager);
            IsMechanicVisible = (CurrentRole == Role.Mechanic);
            IsStorageManagerVisible = (CurrentRole == Role.StorageManager);
        }
        
        private void GotoOrders()
        {
            OnSwitchView("Orders");
        }

        // --- View Switching Logic ---
        public void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    if (CurrentRole == Role.Mechanic)
                    {
                        CurrentContent = _serviceProvider.GetRequiredService<WorkerDashboardViewModel>();
                        if (CurrentContent is WorkerDashboardViewModel vm)
                        {
                            vm.LoadDataAsync();
                        }

                    }
                    else if (CurrentRole == Role.StorageManager)
                    {
                        _storageManagerDashboardViewModel = _serviceProvider.GetRequiredService<StorageManagerDashboardViewModel>();
                        _storageManagerDashboardViewModel.AddPartRequested -= OnRequestCreatePartView;
                        _storageManagerDashboardViewModel.AddPartRequested += OnRequestCreatePartView;
                        CurrentContent = _storageManagerDashboardViewModel;
                    }
                    else if (CurrentRole == Role.Manager)
                    {
                        CurrentContent = _serviceProvider.GetRequiredService<ManagerDashboardViewModel>();

                        if (CurrentContent is ManagerDashboardViewModel vm)
                        {
                            vm.GotoOrders -= GotoOrders;
                            vm.GotoOrders += GotoOrders;
                        }
                    }
                    else if (CurrentRole == Role.Admin) 
                    {
                        CurrentContent = "Admin Dashboard Content Goes Here"; 
                    }
                    else
                    {
                        CurrentContent = "Brak dostępu do pulpitu dla Twojej roli.";
                    }
                    break;
                case "Orders":
                    _ordersViewModel = _serviceProvider.GetRequiredService<OrdersViewModel>();
                    _ordersViewModel.RequestOrderFormView -= OnRequestCreateOrderViewFromList;
                    _ordersViewModel.RequestOrderFormView += OnRequestCreateOrderViewFromList;
                    CurrentContent = _ordersViewModel;
                    break;
                case "Parts": 
                
                    break;
                case "Customers":
                    _clientsListViewModel = _serviceProvider.GetRequiredService<ClientsListViewModel>();
                  
                    _clientsListViewModel.RequestCreateClientView -= OnRequestCreateClientView;
                    _clientsListViewModel.RequestShowClientView -= OnRequestShowClientView;
                    _clientsListViewModel.RequestEditClientView -= OnRequestEditClientView;

                    _clientsListViewModel.RequestCreateClientView += OnRequestCreateClientView;
                    _clientsListViewModel.RequestShowClientView += OnRequestShowClientView;
                    _clientsListViewModel.RequestEditClientView += OnRequestEditClientView;
                    CurrentContent = _clientsListViewModel;
                    break;
                case "Cars":
                    _vehiclesListViewModel = _serviceProvider.GetRequiredService<VehiclesListViewModel>(); 
                    _vehiclesListViewModel.RequestCreateVehicleView -= OnRequestCreateVehicleView; 
                    _vehiclesListViewModel.RequestCreateVehicleView += OnRequestCreateVehicleView;
                    _vehiclesListViewModel.RequestEditVehicleView -= OnRequestEditVehicleView; 
                    _vehiclesListViewModel.RequestEditVehicleView += OnRequestEditVehicleView;
                    
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
                    _createClientFormViewModel.RequestGoBack -= OnEditClientFormGoBack;
                    _createClientFormViewModel.RequestGoBack += OnEditClientFormGoBack;
                    CurrentContent = _createClientFormViewModel;
                    break;
                case "Services": 
                    _servicesListViewModel = _serviceProvider.GetRequiredService<ServicesListViewModel>();
                    
                    _servicesListViewModel.RequestCreateServiceView -= OnRequestCreateServiceViewFromList; 
                    _servicesListViewModel.RequestCreateServiceView += OnRequestCreateServiceViewFromList;
                    _servicesListViewModel.RequestEditServiceView -= OnRequestEditServiceViewFromList;
                    _servicesListViewModel.RequestEditServiceView += OnRequestEditServiceViewFromList;
                    
                    CurrentContent = _servicesListViewModel;
                    break;
                case "CreateService": 
                    _createServiceFormViewModel = _serviceProvider.GetRequiredService<CreateServiceFormViewModel>();
                    _createServiceFormViewModel.ServiceCreated -= OnServiceCreated; 
                    _createServiceFormViewModel.ServiceCreated += OnServiceCreated; 
                    CurrentContent = _createServiceFormViewModel;
                    break;
                case "ShowClient":
                    MessageBox.Show("Nawigacja do szczegółów klienta niezaimplementowana.", "Info");
                    break;
                case "CreatePart":
                    _createPartFormViewModel = _serviceProvider.GetRequiredService<CreatePartFormViewModel>();
                    _createPartFormViewModel.PartCreated -= OnPartCreated;
                    _createPartFormViewModel.PartCreated += OnPartCreated;
                    CurrentContent = _createPartFormViewModel;
                    break;
                default:
                    CurrentContent = "Nieznany widok.";
                    break;
            }
        }

        private void OnEditVehicleInOrderFormGoBack()
        {
            OnSwitchView("Orders");
        }


        private void OnRequestEditVehicleView(Guid vehicleId)
        {
            _editVehicleFormViewModel = _serviceProvider.GetRequiredService<EditVehicleFormViewModel>();
            
            _editVehicleFormViewModel.VehicleUpdated -= OnVehicleUpdated;
            _editVehicleFormViewModel.RequestGoBack -= OnEditVehicleFormGoBack;

            _editVehicleFormViewModel.VehicleUpdated += OnVehicleUpdated;
            _editVehicleFormViewModel.RequestGoBack += OnEditVehicleFormGoBack;

            CurrentContent = _editVehicleFormViewModel; 
            _ = _editVehicleFormViewModel.LoadVehicleAsync(vehicleId); 
        }

        private async void OnVehicleUpdated() 
        {
            MessageBox.Show("Dane pojazdu zostały pomyślnie zaktualizowane! Odświeżam listę pojazdów.", "Sukces");
            OnSwitchView("Cars"); 
            if (_vehiclesListViewModel != null)
            {
                await _vehiclesListViewModel.LoadVehiclesAsync(); 
            }
        }

        private void OnEditVehicleFormGoBack() 
        {
            OnSwitchView("Cars"); 
        }
        private void OnRequestEditServiceViewFromList(Guid serviceId)
        {
            _editServiceFormViewModel = _serviceProvider.GetRequiredService<EditServiceFormViewModel>();

            _editServiceFormViewModel.ServiceUpdated -= OnServiceUpdated;
            _editServiceFormViewModel.RequestGoBack -= OnEditServiceFormGoBack;

            _editServiceFormViewModel.ServiceUpdated += OnServiceUpdated;
            _editServiceFormViewModel.RequestGoBack += OnEditServiceFormGoBack;

            CurrentContent = _editServiceFormViewModel; 
            _ = _editServiceFormViewModel.LoadServiceAsync(serviceId); 
        }
        
        private async void OnServiceUpdated()
        {
            OnSwitchView("Services"); 
            if (_servicesListViewModel != null)
            {
                await _servicesListViewModel.LoadServicesAsync(); 
            }
        }
        private void OnEditServiceFormGoBack()
        {
            OnSwitchView("Services"); 
        }
        private void OnRequestCreateOrderViewFromList(Guid? guid)
        {
           _orderFormViewModel = _serviceProvider.GetRequiredService<OrderFormViewModel>();
            /*_orderFormViewModel.OrderCreated -= OnOrderCreated;
            _orderFormViewModel.OrderCreated += OnOrderCreated;
            _orderFormViewModel.RequestCreateOrderView -= OnRequestCreateOrderViewFromList;
            _orderFormViewModel.RequestCreateOrderView += OnRequestCreateOrderViewFromList;*/
            
            _orderFormViewModel.OnExecuteAddNewVehicle += OnRequestCreateVehicleView;
            CurrentContent = _orderFormViewModel;
        }

        private void OnPartCreated()
        {
            CurrentContent = _storageManagerDashboardViewModel;
            _storageManagerDashboardViewModel.Refresh();
        }
        private void OnRequestCreatePartView()
        {
            OnSwitchView("CreatePart");
        }
        private void OnRequestCreateServiceViewFromList()
        {
            OnSwitchView("CreateService");
        }
        
        private void OnRequestCreateClientView()
        {
            OnSwitchView("CreateClient");
        }

        private void OnRequestCreateVehicleView() 
        {
            OnSwitchView("CreateCar"); 
        }
        private void OnRequestCreateVehicleView(ClientDTO client) 
        {
            OnSwitchView("CreateCar");
            if (CurrentContent is CreateCarFormViewModel vm)
            {
                vm.SelectedClient = client;
            }
        }
        private async void OnCarCreated() 
        {
            MessageBox.Show("Nowy samochód został pomyślnie dodany! Odświeżam listę pojazdów.", "Sukces");
            await (_vehiclesListViewModel?.LoadVehiclesAsync() ?? Task.CompletedTask); // Reload list if it's the current content
            OnSwitchView("Cars"); 
        }

        private async void OnServiceCreated()
        {
            MessageBox.Show("Nowa usługa została pomyślnie dodana! Odświeżam listę usług.", "Sukces");
            OnSwitchView("Services"); 
            if (_servicesListViewModel != null)
            {
                await _servicesListViewModel.LoadServicesAsync(); 
            }
        }

        private void OnRequestShowClientView(Guid clientId)
        {
            MessageBox.Show($"Wyświetl szczegóły klienta o ID: {clientId}", "Info");
        }

        private void OnRequestEditClientView(Guid clientId)
        {
            _editClientFormViewModel = _serviceProvider.GetRequiredService<EditClientFormViewModel>();
            
            _editClientFormViewModel.ClientUpdated -= OnClientUpdated; 
            _editClientFormViewModel.RequestGoBack -= OnEditClientFormGoBack;

            _editClientFormViewModel.ClientUpdated += OnClientUpdated; 
            _editClientFormViewModel.RequestGoBack += OnEditClientFormGoBack;

            CurrentContent = _editClientFormViewModel;
            
            _ = _editClientFormViewModel.LoadClientAsync(clientId); 
        }
        private async void OnClientUpdated()
        {
            MessageBox.Show("Dane klienta zostały pomyślnie zaktualizowane! Odświeżam listę klientów.", "Sukces");
            await (_clientsListViewModel?.LoadClientsAsync() ?? Task.CompletedTask); 
            OnSwitchView("Customers"); 
        }

        private void OnEditClientFormGoBack()
        {
            OnSwitchView("Customers"); 
        }
        private async void OnClientCreated()
        {
            MessageBox.Show("Nowy klient został pomyślnie dodany! Odświeżam listę klientów.", "Sukces");
            await (_clientsListViewModel?.LoadClientsAsync() ?? Task.CompletedTask); 
            OnSwitchView("Customers"); 
        }
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}