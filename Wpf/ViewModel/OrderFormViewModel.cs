using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;


namespace Wpf.ViewModel.Worker
{
    public class OrderFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // --- Services (Dependencies Injected via Constructor) ---
        private readonly OrderService _orderService;
        private readonly ClientService _clientService;
        private readonly VehicleService _vehicleService;
        private readonly ServiceService _serviceService;
        private readonly WorkerService _workerService;
        private readonly SparePartService _sparePartService;

        // --- Private backing lists for filtering ---
        private List<ClientDTO> _allClients = new List<ClientDTO>();
        private List<VehicleDTO> _allVehicles = new List<VehicleDTO>();

        // --- Properties (Formerly from OrderFormModel, now direct) ---
        private OrderDTO _order;
        public OrderDTO Order
        {
            get => _order;
            set
            {
                if (_order != null)
                {
                    _order.ServicesToDo.CollectionChanged -= ServicesToDo_CollectionChanged;
                    foreach (var item in _order.ServicesToDo)
                    {
                        item.PropertyChanged -= ServiceItem_PropertyChanged;
                    }

                    _order.SpareParts.CollectionChanged -= SpareParts_CollectionChanged;
                    foreach (var item in _order.SpareParts)
                    {
                        item.PropertyChanged -= PartItem_PropertyChanged;
                    }
                }

                if (SetField(ref _order, value))
                {
                    if (_order != null)
                    {
                        _order.ServicesToDo.CollectionChanged += ServicesToDo_CollectionChanged;
                        foreach (var item in _order.ServicesToDo)
                        {
                            item.PropertyChanged += ServiceItem_PropertyChanged;
                        }

                        _order.SpareParts.CollectionChanged += SpareParts_CollectionChanged;
                        foreach (var item in _order.SpareParts)
                        {
                            item.PropertyChanged += PartItem_PropertyChanged;
                        }
                    }
                }
            }
        }

        public ObservableCollection<ClientDTO> SearchedClients { get; }
        public ObservableCollection<VehicleDTO> SearchedVehicles { get; }
        public ObservableCollection<ServiceDTO> AvailableServices { get; }
        public ObservableCollection<string> ServiceStatusList { get; }
        public ObservableCollection<WorkerDTO> AvailableMechanics { get; }
        public ObservableCollection<SparePartsDTO> AvailableParts { get; }

        // --- Other Properties ---
        private string _searchCustomersString = string.Empty;
        public string SearchCustomersString
        {
            get => _searchCustomersString;
            set
            {
                if (SetField(ref _searchCustomersString, value))
                {
                    FilterClients();
                }
            }
        }

        private ClientDTO? _selectedClient;
        public ClientDTO? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (SetField(ref _selectedClient, value))
                {
                    ((RelayCommand)AddNewVehicleCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                    FilterVehicles();
                }
            }
        }

        private string _searchVehicleString = string.Empty;
        public string SearchVehicleString
        {
            get => _searchVehicleString;
            set
            {
                if (SetField(ref _searchVehicleString, value))
                {
                    FilterVehicles();
                }
            }
        }

        private VehicleDTO? _selectedVehicle;
        public VehicleDTO? SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                if (SetField(ref _selectedVehicle, value))
                {
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (SetField(ref _isSaving, value))
                {
                    // Te wywołania powinny być na generycznych RelayCommand, ale ok
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)AddNewClientCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)AddNewVehicleCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)AddServiceCommand).RaiseCanExecuteChanged();
                    ((RelayCommand<ServiceInProgressDTO>)RemoveServiceCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)AddPartCommand).RaiseCanExecuteChanged();
                    ((RelayCommand<OrderSparePartDTO>)RemovePartCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public decimal ServicesTotalCost => Order?.ServicesToDo?.Sum(si => si.CalculatedPrice) ?? 0m;
        public decimal PartsTotalCost => Order?.SpareParts?.Sum(spi => spi.TotalItemCost) ?? 0m;
        public decimal TotalCost => ServicesTotalCost + PartsTotalCost;

        public ICommand AddNewClientCommand { get; }
        public ICommand AddNewVehicleCommand { get; }
        public ICommand AddServiceCommand { get; }
        public ICommand RemoveServiceCommand { get; }
        public ICommand AddPartCommand { get; }
        public ICommand RemovePartCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public OrderFormViewModel(OrderService orderService, ClientService clientService, VehicleService vehicleService, ServiceService serviceService, WorkerService workerService, SparePartService sparePartService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _serviceService = serviceService ?? throw new ArgumentNullException(nameof(serviceService));
            _workerService = workerService ?? throw new ArgumentNullException(nameof(workerService));
            _sparePartService = sparePartService ?? throw new ArgumentNullException(nameof(sparePartService));

            _order = new OrderDTO
            {
                ServicesToDo = new ObservableCollection<ServiceInProgressDTO>(),
                SpareParts = new ObservableCollection<OrderSparePartDTO>()
            };

            _order.ServicesToDo.CollectionChanged += ServicesToDo_CollectionChanged;
            _order.SpareParts.CollectionChanged += SpareParts_CollectionChanged;

            SearchedClients = new ObservableCollection<ClientDTO>();
            SearchedVehicles = new ObservableCollection<VehicleDTO>();
            AvailableServices = new ObservableCollection<ServiceDTO>();
            ServiceStatusList = new ObservableCollection<string> { "Nowe", "W trakcie", "Zakończone", "Anulowane" };
            AvailableMechanics = new ObservableCollection<WorkerDTO>();
            AvailableParts = new ObservableCollection<SparePartsDTO>();

            AddNewClientCommand = new RelayCommand(ExecuteAddNewClient, CanExecuteAddNewClient);
            AddNewVehicleCommand = new RelayCommand(ExecuteAddNewVehicle, CanExecuteAddNewVehicle);
            AddServiceCommand = new RelayCommand(ExecuteAddService, CanExecuteAddService);
            RemoveServiceCommand = new RelayCommand<ServiceInProgressDTO>(ExecuteRemoveService, CanExecuteRemoveService);
            AddPartCommand = new RelayCommand(ExecuteAddPart, CanExecuteAddPart);
            RemovePartCommand = new RelayCommand<OrderSparePartDTO>(ExecuteRemovePart, CanExecuteRemovePart);
            CancelCommand = new RelayCommand(ExecuteCancel, CanExecuteCancel);
            SaveCommand = new RelayCommand(async () => await ExecuteSave(), CanExecuteSave);

            _ = LoadData();
        }

        // --- Handlery zdarzeń kolekcji i elementów (bez zmian) ---
        private void ServicesToDo_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ServiceInProgressDTO item in e.OldItems)
                {
                    item.PropertyChanged -= ServiceItem_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ServiceInProgressDTO item in e.NewItems)
                {
                    item.PropertyChanged += ServiceItem_PropertyChanged;
                }
            }
            OnPropertyChanged(nameof(ServicesTotalCost));
            OnPropertyChanged(nameof(TotalCost));
        }

        private void SpareParts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (OrderSparePartDTO item in e.OldItems)
                {
                    item.PropertyChanged -= PartItem_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (OrderSparePartDTO item in e.NewItems)
                {
                    item.PropertyChanged += PartItem_PropertyChanged;
                }
            }
            OnPropertyChanged(nameof(PartsTotalCost));
            OnPropertyChanged(nameof(TotalCost));
        }

        private void ServiceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServiceInProgressDTO.Price) || e.PropertyName == nameof(ServiceInProgressDTO.Service))
            {
                OnPropertyChanged(nameof(ServicesTotalCost));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private void PartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderSparePartDTO.Quantity) || e.PropertyName == nameof(OrderSparePartDTO.SparePart))
            {
                OnPropertyChanged(nameof(PartsTotalCost));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        // --- Data Loading and Filtering Methods (bez zmian) ---
        private async Task LoadData()
        {
            try
            {
                _allClients.Clear();
                var clients = await _clientService.GetClientsAsync();
                if (clients != null)
                {
                    _allClients.AddRange(clients);
                }
                FilterClients();

                _allVehicles.Clear();
                var vehicles = await _vehicleService.GetVehiclesAsync();
                if (vehicles != null)
                {
                    _allVehicles.AddRange(vehicles);
                }
                FilterVehicles();

                AvailableServices.Clear();
                var services = await _serviceService.GetServicesAsync();
                if (services != null)
                {
                    foreach (var service in services)
                    {
                        AvailableServices.Add(service);
                    }
                }

                AvailableMechanics.Clear();
                var mechanics = await _workerService.GetWorkersAsync();
                if (mechanics != null)
                {
                    foreach (var mechanic in mechanics)
                    {
                        AvailableMechanics.Add(mechanic);
                    }
                }

                AvailableParts.Clear();
                var spareParts = await _sparePartService.GetSparePartsAsync();
                if (spareParts != null)
                {
                    foreach (var part in spareParts)
                    {
                        AvailableParts.Add(part);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterClients()
        {
            SearchedClients.Clear();
            if (string.IsNullOrWhiteSpace(SearchCustomersString))
            {
                foreach (var client in _allClients)
                {
                    SearchedClients.Add(client);
                }
            }
            else
            {
                var searchLower = SearchCustomersString.ToLowerInvariant();
                foreach (var client in _allClients.Where(c =>
                    (c.FirstName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (c.LastName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (c.PhoneNumber?.Contains(searchLower) ?? false) ||
                    (c.Email?.ToLowerInvariant().Contains(searchLower) ?? false)))
                {
                    SearchedClients.Add(client);
                }
            }
        }

        private void FilterVehicles()
        {
            SearchedVehicles.Clear();

            if (SelectedClient == null)
            {
                return;
            }

            var vehiclesToFilter = _allVehicles.Where(v => v.ClientId == SelectedClient.Id.ToString());

            if (!string.IsNullOrWhiteSpace(SearchVehicleString))
            {
                var searchLower = SearchVehicleString.ToLowerInvariant();
                vehiclesToFilter = vehiclesToFilter.Where(v =>
                    (v.Make?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (v.Model?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (v.RegistrationNumber?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                    (v.Vin?.ToLowerInvariant().Contains(searchLower) ?? false));
            }

            foreach (var vehicle in vehiclesToFilter)
            {
                SearchedVehicles.Add(vehicle);
            }
        }

        // --- Command Methods ---
        private void ExecuteAddNewClient()
        {
            Console.WriteLine("Execute: Add New Client");
            MessageBox.Show("Otwieranie formularza dodawania nowego klienta...", "Dodaj Klienta", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecuteAddNewClient()
        {
            return !IsSaving;
        }

        private void ExecuteAddNewVehicle()
        {
            Console.WriteLine("Execute: Add New Vehicle");
            MessageBox.Show("Otwieranie formularza dodawania nowego pojazdu...", "Dodaj Pojazd", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecuteAddNewVehicle()
        {
            return SelectedClient != null && !IsSaving;
        }

        private void ExecuteAddService()
        {
            Console.WriteLine("Execute: Add Service");
            var newService = new ServiceInProgressDTO { ServiceStatus = ServiceStatus.PendingForStart };
            Order.ServicesToDo.Add(newService);
            newService.PropertyChanged += ServiceItem_PropertyChanged;
            OnPropertyChanged(nameof(ServicesTotalCost));
            OnPropertyChanged(nameof(TotalCost));
        }

        private bool CanExecuteAddService()
        {
            return !IsSaving;
        }

        private void ExecuteRemoveService(ServiceInProgressDTO serviceToRemove)
        {
            if (serviceToRemove != null)
            {
                Console.WriteLine($"Execute: Remove Service - {serviceToRemove.Service?.Name}");
                Order.ServicesToDo.Remove(serviceToRemove);
                serviceToRemove.PropertyChanged -= ServiceItem_PropertyChanged;
                OnPropertyChanged(nameof(ServicesTotalCost));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private bool CanExecuteRemoveService(ServiceInProgressDTO serviceToRemove)
        {
            return serviceToRemove != null && !IsSaving;
        }

        private void ExecuteAddPart()
        {
            Console.WriteLine("Execute: Add Part");
            var newPart = new OrderSparePartDTO { Quantity = 1 };
            Order.SpareParts.Add(newPart);
            newPart.PropertyChanged += PartItem_PropertyChanged;
            OnPropertyChanged(nameof(PartsTotalCost));
            OnPropertyChanged(nameof(TotalCost));
        }

        private bool CanExecuteAddPart()
        {
            return !IsSaving;
        }

        private void ExecuteRemovePart(OrderSparePartDTO partToRemove)
        {
            if (partToRemove != null)
            {
                Console.WriteLine($"Execute: Remove Part - {partToRemove.Id}");
                Order.SpareParts.Remove(partToRemove);
                partToRemove.PropertyChanged -= PartItem_PropertyChanged;
                OnPropertyChanged(nameof(PartsTotalCost));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private bool CanExecuteRemovePart(OrderSparePartDTO partToRemove)
        {
            return partToRemove != null && !IsSaving;
        }

        private void ExecuteCancel()
        {
            Console.WriteLine("Execute: Cancel");
            MessageBox.Show("Tworzenie zlecenia anulowane.", "Anuluj", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
        }

        private bool CanExecuteCancel()
        {
            return !IsSaving;
        }

        private async Task ExecuteSave()
        {
            IsSaving = true;
            try
            {
                // ZMIANA: Dodano walidację przed wywołaniem CanExecuteSave
                if (SelectedClient == null)
                {
                    MessageBox.Show("Proszę wybrać klienta przed zapisaniem zlecenia.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (SelectedVehicle == null)
                {
                    MessageBox.Show("Proszę wybrać pojazd przed zapisaniem zlecenia.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ZMIANA: Walidacja, czy wybrano usługi katalogowe dla pozycji usług
                if (Order.ServicesToDo.Any(s => s.Service == null || s.Service.Id == Guid.Empty)) // Upewnij się, że Service jest i jego Id nie jest puste
                {
                    MessageBox.Show("Każda dodana usługa musi mieć wybraną usługę katalogową (np. 'Wymiana oleju'). Proszę wybrać usługę z listy.", "Błąd Walidacji Usług", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // ZMIANA: Walidacja, czy wybrano części katalogowe dla pozycji części
                if (Order.SpareParts.Any(p => p.SparePart == null || p.SparePart.Id == Guid.Empty)) // Upewnij się, że SparePart jest i jego Id nie jest puste
                {
                    MessageBox.Show("Każda dodana część musi mieć wybraną część katalogową (np. 'Filtr oleju'). Proszę wybrać część z listy.", "Błąd Walidacji Części", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // Upewnij się, że CanExecuteSave bierze pod uwagę nowe walidacje, jeśli jest wywoływane w innym miejscu
                /*
                if (!CanExecuteSave()) // Ta linia może być redundantna, jeśli walidacje są już wyżej
                {
                    // Tutaj można dodać ogólny komunikat, jeśli CanExecuteSave zwraca false z innego powodu
                    MessageBox.Show("Nie można zapisać zlecenia. Sprawdź, czy wszystkie wymagane pola są wypełnione i czy nie ma błędów walidacji.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                */

                Order.ClientId = SelectedClient.Id.ToString();
                Order.VehicleId = SelectedVehicle.Id.ToString();
                var (resultOrder, errorMessage) = await _orderService.CreateOrderAsync(Order);

                if (resultOrder != null)
                {
                    MessageBox.Show("Zlecenie zostało pomyślnie zapisane!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show($"Błąd podczas zapisywania zlecenia: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanExecuteSave()
        {
            // ZMIANA: Dodano walidacje do CanExecuteSave
            return SelectedClient != null && SelectedVehicle != null && !IsSaving &&
                   !Order.ServicesToDo.Any(s => s.Service == null || s.Service.Id == Guid.Empty) && // Wszystkie usługi muszą mieć wybraną usługę
                   !Order.SpareParts.Any(p => p.SparePart == null || p.SparePart.Id == Guid.Empty); // Wszystkie części muszą mieć wybraną część
        }

        private void ClearForm()
        {
            Order = new OrderDTO
            {
                ServicesToDo = new ObservableCollection<ServiceInProgressDTO>(),
                SpareParts = new ObservableCollection<OrderSparePartDTO>()
            };
            SelectedClient = null;
            SelectedVehicle = null;
            SearchCustomersString = string.Empty;
            SearchVehicleString = string.Empty;
            FilterClients();
            FilterVehicles();
        }

        // --- INotifyPropertyChanged Implementation (bez zmian) ---
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddNewClientCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddNewVehicleCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddServiceCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AddPartCommand).RaiseCanExecuteChanged();
            ((RelayCommand<ServiceInProgressDTO>)RemoveServiceCommand).RaiseCanExecuteChanged();
            ((RelayCommand<OrderSparePartDTO>)RemovePartCommand).RaiseCanExecuteChanged();
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}