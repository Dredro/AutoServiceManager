using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models;
using Wpf.Models.DTOs;

namespace Wpf.ViewModel.Worker;

public class OrderFormViewModel : INotifyPropertyChanged
{
    #region Private Fields
    private string _searchCustomersString = string.Empty;
    private string _searchVehicleString = string.Empty;
    private ClientDTO? _selectedClient;
    private VehicleDTO? _selectedVehicle;
    private WorkerDTO? _selectedMechanic;
    private OrderFormModel _model;
    #endregion

    #region Properties
    public OrderFormModel Model
    {
        get => _model;
        set
        {
            _model = value;
            OnPropertyChanged();
        }
    }

    public string SearchCustomersString
    {
        get => _searchCustomersString;
        set
        {
            _searchCustomersString = value;
            OnPropertyChanged();
            SearchClients();
        }
    }

    public string SearchVehicleString
    {
        get => _searchVehicleString;
        set
        {
            _searchVehicleString = value;
            OnPropertyChanged();
            SearchVehicles();
        }
    }

    public ClientDTO? SelectedClient
    {
        get => _selectedClient;
        set
        {
            _selectedClient = value;
            OnPropertyChanged();
            if (value != null)
            {
                Model.Order.Client = value;
                LoadClientVehicles();
            }
            else
            {
                Model.SearchedVehicles.Clear();
                SelectedVehicle = null;
            }
        }
    }

    public VehicleDTO? SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            _selectedVehicle = value;
            OnPropertyChanged();
            if (value != null)
            {
                Model.Order.Vehicle = value;
            }
        }
    }

    public WorkerDTO? SelectedMechanic
    {
        get => _selectedMechanic;
        set
        {
            _selectedMechanic = value;
            OnPropertyChanged();
        }
    }

    public string Title { get; set; } = "Formularz zlecenia";
    public bool IsEditMode { get; private set; }
    #endregion

    #region Commands
    public ICommand AddServiceCommand { get; }
    public ICommand RemoveServiceCommand { get; }
    public ICommand AddPartCommand { get; }
    public ICommand RemovePartCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    #endregion

    #region Constructor
    public OrderFormViewModel(OrderDTO? existingOrder = null)
    {
        IsEditMode = existingOrder != null;
        Title = IsEditMode ? "Edytuj zlecenie" : "Dodaj nowe zlecenie";
        _model = new OrderFormModel();

        if (existingOrder != null)
        {
            // Tryb edycji - wczytaj istniejące zlecenie
            LoadExistingOrder(existingOrder);
        }
        else
        {
            // Tryb dodawania nowego zlecenia


            var order = new OrderDTO(){
                Id = Guid.NewGuid(),
                ServicesToDo = new ObservableCollection<ServiceInProgressDTO>(),
                SpareParts = new ObservableCollection<OrderSparePartDTO>()
            };

            _model.Order = order;
        }

        // Initialize commands
        AddServiceCommand = new RelayCommand(AddService);
        RemoveServiceCommand = new RelayCommand<ServiceInProgressDTO>(RemoveService);
        AddPartCommand = new RelayCommand(AddPart);
        RemovePartCommand = new RelayCommand<OrderSparePartDTO>(RemovePart);
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);

        // Load initial data
        LoadInitialData();
    }
    #endregion

    private void LoadExistingOrder(OrderDTO existingOrder)
    {
        Model.Order = existingOrder;

        // Wczytaj klienta i pojazd
        if (existingOrder.Client != null)
        {
            SelectedClient = existingOrder.Client;
            Model.SearchedClients.Add(existingOrder.Client);
        }

        if (existingOrder.Vehicle != null)
        {
            SelectedVehicle = existingOrder.Vehicle;
            Model.SearchedVehicles.Add(existingOrder.Vehicle);
        }
    }

    #region Methods
    private void LoadInitialData()
    {
        // Load available services, parts, mechanics, etc.
        // This would typically come from a service/repository
        LoadAvailableServices();
        LoadAvailableParts();
        LoadAvailableMechanics();
        LoadServiceStatuses();
    }

    private void LoadAvailableServices()
    {
        // Example implementation - replace with actual service call
        Model.AvailableServices = new ObservableCollection<ServiceDTO>
            {
                new ServiceDTO { Name = "Wymiana oleju", Description = "Wymiana oleju silnikowego", MinimalPrice = 100, MaximalPrice = 200 },
                new ServiceDTO { Name = "Wymiana klocków hamulcowych", Description = "Wymiana klocków hamulcowych", MinimalPrice = 150, MaximalPrice = 300 },
                new ServiceDTO { Name = "Przegląd techniczny", Description = "Przegląd techniczny pojazdu", MinimalPrice = 50, MaximalPrice = 100 }
            };
    }

    private void LoadAvailableParts()
    {
        // Example implementation - replace with actual service call
        Model.AvailableParts = new ObservableCollection<SparePartsDTO>
            {
                new SparePartsDTO { CatalogNumber = "OIL001", Name = "Olej silnikowy 5W-30", Make = "Castrol", Quality = 'A', Price = 45.99m, Category = PartCategory.Consumables, QuantityInStock = 50 },
                new SparePartsDTO { CatalogNumber = "BRK001", Name = "Klocki hamulcowe przód", Make = "Bosch", Quality = 'A', Price = 89.99m, Category = PartCategory.BrakingSystem, QuantityInStock = 25 },
                new SparePartsDTO { CatalogNumber = "FLT001", Name = "Filtr oleju", Make = "Mann", Quality = 'A', Price = 25.99m, Category = PartCategory.Consumables, QuantityInStock = 100 }
            };
    }

    private void LoadAvailableMechanics()
    {
        // Example implementation - replace with actual service call
        Model.AvailableMechanics = new ObservableCollection<WorkerDTO>
            {
                new WorkerDTO { PersonalInfo = new PersonalInfo { FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com", PhoneNumber = "123456789" } },
                new WorkerDTO { PersonalInfo = new PersonalInfo { FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com", PhoneNumber = "987654321" } }
            };
    }

    private void LoadServiceStatuses()
    {
        Model.ServiceStatusList = new ObservableCollection<ServiceStatus>
            {
                ServiceStatus.PendingForStart,
                ServiceStatus.PendingForParts,
                ServiceStatus.InProgress,
                ServiceStatus.Completed
            };
    }

    private void SearchClients()
    {
        if (string.IsNullOrWhiteSpace(SearchCustomersString))
        {
            Model.SearchedClients.Clear();
            return;
        }

        // Example implementation - replace with actual search logic
        var allClients = GetAllClients(); // This would come from a service
        var filtered = allClients.Where(c =>
            c.PersonalInfo?.FirstName?.Contains(SearchCustomersString, StringComparison.OrdinalIgnoreCase) == true ||
            c.PersonalInfo?.LastName?.Contains(SearchCustomersString, StringComparison.OrdinalIgnoreCase) == true ||
            c.PersonalInfo?.Email?.Contains(SearchCustomersString, StringComparison.OrdinalIgnoreCase) == true ||
            c.PersonalInfo?.PhoneNumber?.Contains(SearchCustomersString, StringComparison.OrdinalIgnoreCase) == true
        ).ToList();

        Model.SearchedClients.Clear();
        foreach (var client in filtered)
        {
            Model.SearchedClients.Add(client);
        }
    }

    private void SearchVehicles()
    {
        if (SelectedClient == null || string.IsNullOrWhiteSpace(SearchVehicleString))
        {
            if (SelectedClient != null)
            {
                LoadClientVehicles();
            }
            return;
        }

        var filtered = SelectedClient.Vehicles.Where(v =>
            v.Make?.Contains(SearchVehicleString, StringComparison.OrdinalIgnoreCase) == true ||
            v.Model?.Contains(SearchVehicleString, StringComparison.OrdinalIgnoreCase) == true ||
            v.RegistrationNumber?.Contains(SearchVehicleString, StringComparison.OrdinalIgnoreCase) == true
        ).ToList();

        Model.SearchedVehicles.Clear();
        foreach (var vehicle in filtered)
        {
            Model.SearchedVehicles.Add(vehicle);
        }
    }

    private void LoadClientVehicles()
    {
        if (SelectedClient == null) return;

        Model.SearchedVehicles.Clear();
        foreach (var vehicle in SelectedClient.Vehicles)
        {
            Model.SearchedVehicles.Add(vehicle);
        }
    }

    private List<ClientDTO> GetAllClients()
    {
        // Example data - replace with actual service call
        return new List<ClientDTO>
            {
                new ClientDTO
                {
                    Id = Guid.NewGuid(),
                    PersonalInfo = new PersonalInfo { FirstName = "Jan", LastName = "Kowalski", Email = "jan@example.com", PhoneNumber = "123456789" },
                    Vehicles = new List<VehicleDTO>
                    {
                        new VehicleDTO { Make = "Toyota", Model = "Corolla", RegistrationNumber = "ABC123", YearOfProduction = 2020 },
                        new VehicleDTO { Make = "Ford", Model = "Focus", RegistrationNumber = "XYZ789", YearOfProduction = 2019 }
                    }
                },
                new ClientDTO
                {
                    Id = Guid.NewGuid(),
                    PersonalInfo = new PersonalInfo { FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com", PhoneNumber = "987654321" },
                    Vehicles = new List<VehicleDTO>
                    {
                        new VehicleDTO { Make = "BMW", Model = "X3", RegistrationNumber = "DEF456", YearOfProduction = 2021 }
                    }
                }
            };
    }

    private void AddService()
    {
        var newService = new ServiceInProgressDTO
        {
            Service = Model.AvailableServices.FirstOrDefault() ?? new ServiceDTO { Name = "Nowa usługa", MinimalPrice = 0, MaximalPrice = 0 },
            ServiceStatus = ServiceStatus.PendingForStart,
            Order = Model.Order
        };

        Model.Order.ServicesToDo.Add(newService);
        Model.UpdateTotalCosts();
    }

    private void RemoveService(ServiceInProgressDTO? service)
    {
        if (service != null && Model.Order.ServicesToDo.Contains(service))
        {
            Model.Order.ServicesToDo.Remove(service);
            Model.UpdateTotalCosts();
        }
    }

    private void AddPart()
    {
        var newPart = new OrderSparePartDTO
        {
            OrderId = Guid.NewGuid(),
            Order = Model.Order,
            SparePart = Model.AvailableParts.FirstOrDefault(),
            Quantity = 1
        };

        Model.Order.SpareParts.Add(newPart);
        Model.UpdateTotalCosts();
    }

    private void RemovePart(OrderSparePartDTO? part)
    {
        if (part != null && Model.Order.SpareParts.Contains(part))
        {
            Model.Order.SpareParts.Remove(part);
            Model.UpdateTotalCosts();
        }
    }

    private bool CanSave()
    {
        return SelectedClient != null &&
               SelectedVehicle != null &&
               Model.Order.ServicesToDo.Any();
    }

    private void Save()
    {
        if (!CanSave())
        {
            MessageBox.Show("Proszę wypełnić wszystkie wymagane pola", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Model.Order.Client = SelectedClient;
        Model.Order.Vehicle = SelectedVehicle;
        Model.UpdateTotalCosts();

        CloseWindow(true);
    }

    private void Cancel()
    {
        CloseWindow(false);
    }

    private void CloseWindow(bool dialogResult)
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.DialogResult = dialogResult;
                window.Close();
                break;
            }
        }
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}