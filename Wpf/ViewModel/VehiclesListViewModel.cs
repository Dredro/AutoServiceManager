using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;
using Wpf.ViewModels; 

namespace Wpf.ViewModels 
{
    public class VehiclesListViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private readonly ClientService _clientService; 

        public event PropertyChangedEventHandler? PropertyChanged; 
        
        
        public event Action? RequestCreateVehicleView; 

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<VehicleDTO> _vehicles;
        public ObservableCollection<VehicleDTO> Vehicles
        {
            get => _vehicles;
            set
            {
                if (_vehicles != value) 
                {
                    _vehicles = value;
                    OnPropertyChanged(nameof(Vehicles));
                }
            }
        }

        private bool _isLoading;
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value) 
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    
                    ((RelayCommand)AddNewVehicleCommand).RaiseCanExecuteChanged();
                }
            }
        }

        
        public ICommand AddNewVehicleCommand { get; private set; }
        public ICommand ShowVehicleCommand { get; private set; }
        public ICommand EditVehicleCommand { get; private set; }
        public ICommand DeleteVehicleCommand { get; private set; }

        public VehiclesListViewModel(VehicleService vehicleService, ClientService clientService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService)); 

            Vehicles = new ObservableCollection<VehicleDTO>();

            
            AddNewVehicleCommand = new RelayCommand(OnAddNewVehicle, () => !IsLoading);

            ShowVehicleCommand = new RelayCommand<VehicleDTO>(OnShowVehicle, CanExecuteVehicleAction);
            EditVehicleCommand = new RelayCommand<VehicleDTO>(OnEditVehicle, CanExecuteVehicleAction);
            DeleteVehicleCommand = new RelayCommand<VehicleDTO>(async (vehicle) => await OnDeleteVehicle(vehicle), CanExecuteVehicleAction); 
           
            _ = LoadVehiclesAsync();
        }

        public async Task LoadVehiclesAsync() 
        {
            IsLoading = true;
            try
            {
                var loadedVehicles = await _vehicleService.GetVehiclesAsync();
                Vehicles.Clear(); 
                if (loadedVehicles != null)
                {
                    foreach (var vehicle in loadedVehicles)
                    {
                        vehicle.Client = await _clientService.GetClientByIdAsync(vehicle.ClientId);
                        Vehicles.Add(vehicle);
                    }
                }
                else
                {
                    MessageBox.Show("Nie udało się załadować listy pojazdów.", "Błąd ładowania", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania pojazdów: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAddNewVehicle()
        {
            RequestCreateVehicleView?.Invoke(); 
        }

        private bool CanExecuteVehicleAction(VehicleDTO? vehicle) 
        {
            return vehicle != null && !IsLoading; 
        }

        private void OnShowVehicle(VehicleDTO? vehicle) 
        {
            if (vehicle != null) 
            {
                MessageBox.Show($"Wyświetl pojazd: {vehicle.Make} {vehicle.Model} (ID: {vehicle.Id})", "Pokaż pojazd", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }
        
        private void OnEditVehicle(VehicleDTO? vehicle) 
        {
            if (vehicle != null)
            {
                MessageBox.Show($"Edytuj pojazd: {vehicle.Make} {vehicle.Model} (ID: {vehicle.Id})", "Edytuj pojazd", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }

        private async Task OnDeleteVehicle(VehicleDTO? vehicle) 
        {
            if (vehicle == null) return;

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć pojazd: {vehicle.Make} {vehicle.Model} (ID: {vehicle.Id})?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true; 
                try
                {
                    MessageBox.Show($"Pojazd {vehicle.Make} {vehicle.Model} usunięty (symulacja).", "Usunięto", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadVehiclesAsync(); 

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas usuwania pojazdu: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}