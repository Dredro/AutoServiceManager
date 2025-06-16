using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows; 
using System.Windows.Input;

using Wpf.Core;      
using Wpf.Services;   
using Wpf.Models.DTOs; 


namespace Wpf.ViewModels
{
    public class CreateCarFormViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private readonly ClientService _clientService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? CarCreated; 
        public event Action? RequestGoBack;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private static readonly Dictionary<string, VehicleType> VehicleTypeMap = new Dictionary<string, VehicleType>
        {
            { "Samochód osobowy", VehicleType.Car },
            { "Motocykl", VehicleType.Motorcycle },
            { "Ciężarówka", VehicleType.Truck }
        };

        public ObservableCollection<string> VehicleTypes { get; }

        private string _selectedVehicleType = "Samochód osobowy";
        public string SelectedVehicleType
        {
            get => _selectedVehicleType;
            set
            {
                if (_selectedVehicleType != value)
                {
                    _selectedVehicleType = value;
                    OnPropertyChanged(nameof(SelectedVehicleType));
                }
            }
        }

        private string _make = string.Empty;
        public string Make
        {
            get => _make;
            set
            {
                if (_make != value)
                {
                    _make = value;
                    OnPropertyChanged(nameof(Make));
                }
            }
        }

        private string _model = string.Empty;
        public string Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged(nameof(Model));
                }
            }
        }

        private string _vin = string.Empty;
        public string Vin
        {
            get => _vin;
            set
            {
                if (_vin != value)
                {
                    _vin = value;
                    OnPropertyChanged(nameof(Vin));
                }
            }
        }

        private string? _registrationNumber;
        public string? RegistrationNumber
        {
            get => _registrationNumber;
            set
            {
                if (_registrationNumber != value)
                {
                    _registrationNumber = value;
                    OnPropertyChanged(nameof(RegistrationNumber));
                }
            }
        }

        private DateTime? _registrationDate = DateTime.Now;
        public DateTime? RegistrationDate
        {
            get => _registrationDate;
            set
            {
                if (_registrationDate != value)
                {
                    _registrationDate = value;
                    OnPropertyChanged(nameof(RegistrationDate));
                }
            }
        }

        private int? _yearOfProduction = DateTime.Now.Year; 
        public int? YearOfProduction
        {
            get => _yearOfProduction;
            set
            {
                if (_yearOfProduction != value)
                {
                    _yearOfProduction = value;
                    OnPropertyChanged(nameof(YearOfProduction));
                }
            }
        }

        private string? _engineCode;
        public string? EngineCode
        {
            get => _engineCode;
            set
            {
                if (_engineCode != value)
                {
                    _engineCode = value;
                    OnPropertyChanged(nameof(EngineCode));
                }
            }
        }

        private int? _engineDisplacement = 1000; 
        public int? EngineDisplacement
        {
            get => _engineDisplacement;
            set
            {
                if (_engineDisplacement != value)
                {
                    _engineDisplacement = value;
                    OnPropertyChanged(nameof(EngineDisplacement));
                }
            }
        }

        private decimal? _power = 75.0M; 
        public decimal? Power
        {
            get => _power;
            set
            {
                if (_power != value)
                {
                    _power = value;
                    OnPropertyChanged(nameof(Power));
                }
            }
        }
        
        public ObservableCollection<ClientDTO> Clients { get; } = new ObservableCollection<ClientDTO>();
        public ObservableCollection<ClientDTO> FilteredClients { get; } = new ObservableCollection<ClientDTO>();

        private ClientDTO? _selectedClient;
        public ClientDTO? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    OnPropertyChanged(nameof(SelectedClient));
                    
                    if (_selectedClient != null)
                    {
                        string displayString = _selectedClient.ToString(); 
                        if (_clientSearchText != displayString)
                        {
                            _clientSearchText = displayString;
                            OnPropertyChanged(nameof(ClientSearchText));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_clientSearchText))
                        {
                            _clientSearchText = string.Empty;
                            OnPropertyChanged(nameof(ClientSearchText));
                        }
                    }
                }
            }
        }

        private string _clientSearchText = string.Empty;
        public string ClientSearchText
        {
            get => _clientSearchText;
            set
            {
                if (_clientSearchText != value)
                {
                    _clientSearchText = value;
                    OnPropertyChanged(nameof(ClientSearchText));
                    FilterClients(); 
                    
                    if (_selectedClient != null)
                    {
                        string selectedClientDisplayName = _selectedClient.ToString(); 
                        if (!value.Equals(selectedClientDisplayName, StringComparison.OrdinalIgnoreCase))
                        {
                            _selectedClient = null;
                            OnPropertyChanged(nameof(SelectedClient));
                        }
                    }
                }
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    OnPropertyChanged(nameof(IsSaving));
                }
            }
        }

        private bool _isLoadingClients;
        public bool IsLoadingClients
        {
            get => _isLoadingClients;
            set
            {
                if (_isLoadingClients != value)
                {
                    _isLoadingClients = value;
                    OnPropertyChanged(nameof(IsLoadingClients));
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public CreateCarFormViewModel(VehicleService vehicleService, ClientService clientService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));

            VehicleTypes = new ObservableCollection<string>
            {
                "Samochód osobowy",
                "Motocykl",
                "Ciężarówka"
            };

            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave);
            CancelCommand = new RelayCommand(OnCancel);

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadClientsAsync();
            if (Clients.Any() && SelectedClient == null)
            {
                SelectedClient = Clients.FirstOrDefault();
            }
        }

        private async Task LoadClientsAsync()
        {
            IsLoadingClients = true;
            try
            {
                var loadedClients = await _clientService.GetClientsAsync();
                if (loadedClients != null)
                {
                    Clients.Clear();
                    foreach (var client in loadedClients)
                    {
                        Clients.Add(client);
                    }
                    FilterClients(); 
                }
                else
                {
                    MessageBox.Show("Nie udało się załadować listy klientów.", "Błąd ładowania", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania klientów: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoadingClients = false;
            }
        }

        private void FilterClients()
        {
            if (Clients == null) return;

            var lowerSearchText = ClientSearchText?.ToLowerInvariant();

            var filtered = string.IsNullOrWhiteSpace(lowerSearchText)
                ? Clients.ToList()
                : Clients.Where(c => (
                                     (c.FirstName?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                                     (c.LastName?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                                     c.Id.ToString().ToLowerInvariant().Contains(lowerSearchText)
                                     )).ToList();

            FilteredClients.Clear();
            foreach (var client in filtered)
            {
                FilteredClients.Add(client);
            }
        }

        // --- Command Execution Logic ---
        private async Task OnSave()
        {
            if (IsSaving || IsLoadingClients) return;

            IsSaving = true;
            try
            {
                if (!CanSave())
                {
                    MessageBox.Show("Proszę wypełnić wszystkie wymagane pola i sprawdzić poprawność danych.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!VehicleTypeMap.TryGetValue(SelectedVehicleType, out var vehicleTypeEnum))
                {
                    MessageBox.Show("Niepoprawny rodzaj pojazdu.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedClient == null)
                {
                    MessageBox.Show("Proszę wybrać klienta.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var command = new CreateVehicleCommand
                {
                    Make = Make,
                    Model = Model,
                    Vin = Vin,
                    RegistrationNumber = RegistrationNumber,
                    RegistrationDate = DateOnly.FromDateTime(RegistrationDate!.Value),
                    YearOfProduction = YearOfProduction!.Value,
                    EngineCode = EngineCode,
                    EngineDisplacement = EngineDisplacement!.Value,
                    Power = Power!.Value,
                    ClientId = SelectedClient.Id.ToString(),
                    Type = vehicleTypeEnum
                };

                var (resultId, errorMessage) = await _vehicleService.CreateVehicleAsync(command);

                if (resultId != null)
                {
                    MessageBox.Show($"Samochód '{command.Make} {command.Model}' (ID: {resultId}) został pomyślnie dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    CarCreated?.Invoke();
                }
                else
                {
                    MessageBox.Show($"Błąd podczas dodawania samochodu: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private bool CanSave()
        {
            /*return !IsSaving && !IsLoadingClients &&
                   SelectedClient != null &&
                   !string.IsNullOrWhiteSpace(SelectedVehicleType) && VehicleTypeMap.ContainsKey(SelectedVehicleType) &&
                   !string.IsNullOrWhiteSpace(Make) &&
                   !string.IsNullOrWhiteSpace(Model) &&
                   !string.IsNullOrWhiteSpace(Vin) && Vin?.Length == 17 &&
                   RegistrationDate.HasValue &&
                   YearOfProduction.HasValue && YearOfProduction.Value >= 1900 && YearOfProduction.Value <= DateTime.Now.Year + 1 &&
                   EngineDisplacement.HasValue && EngineDisplacement.Value > 0 &&
                   Power.HasValue && Power.Value > 0;*/
            return true;
        }

        private void OnCancel()
        {
            if (MessageBox.Show("Czy na pewno chcesz anulować edycję? Niezapisane zmiany zostaną utracone.", "Anuluj Edycję", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RequestGoBack?.Invoke(); 
            }
        }

        private void ClearForm()
        {
            SelectedVehicleType = "Samochód osobowy";
            Make = string.Empty;
            Model = string.Empty;
            Vin = string.Empty;
            RegistrationNumber = null;
            RegistrationDate = DateTime.Now;
            YearOfProduction = DateTime.Now.Year;
            EngineCode = null;
            EngineDisplacement = 1000;
            Power = 75.0M;
            ClientSearchText = string.Empty;
            SelectedClient = null;
        }
    }
}