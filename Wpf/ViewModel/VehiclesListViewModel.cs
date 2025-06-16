using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;

namespace Wpf.ViewModels
{
    public class VehiclesListViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestCreateVehicleView; 
        public event Action<Guid>? RequestEditVehicleView;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<VehicleDTO> Vehicles { get; } = new ObservableCollection<VehicleDTO>();

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
                }
            }
        }
        public ICommand AddVehicleCommand { get; }
        public ICommand LoadVehiclesCommand { get; }
        public ICommand CreateVehicleCommand { get; }
        public ICommand EditVehicleCommand { get; } 
        public ICommand DeleteVehicleCommand { get; }

        public VehiclesListViewModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            AddVehicleCommand = new RelayCommand(OnAddNewVehicle);
            LoadVehiclesCommand = new RelayCommand(async () => await LoadVehiclesAsync());
            CreateVehicleCommand = new RelayCommand(() => RequestCreateVehicleView?.Invoke());
            
            EditVehicleCommand = new RelayCommand<VehicleDTO>(OnEditVehicle); 
            
            DeleteVehicleCommand = new RelayCommand<VehicleDTO>(async (vehicle) => await OnDeleteVehicle(vehicle)); 

            _ = LoadVehiclesAsync(); 
        }

        public async Task LoadVehiclesAsync()
        {
            IsLoading = true;
            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync();
                if (vehicles != null)
                {
                    Vehicles.Clear();
                    foreach (var vehicle in vehicles)
                    {
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
        private void OnEditVehicle(VehicleDTO? vehicle) 
        {
            if (vehicle != null)
            {
                RequestEditVehicleView?.Invoke(vehicle.Id); 
            }
        }

        private async Task OnDeleteVehicle(VehicleDTO vehicle) 
        {
            if (MessageBox.Show($"Czy na pewno chcesz usunąć pojazd: {vehicle.Model}?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var (success, errorMessage) = await _vehicleService.DeleteVehicleAsync(vehicle.Id.ToString()); 
                if (success)
                {
                    MessageBox.Show("Pojazd usunięto pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadVehiclesAsync(); 
                }
                else
                {
                    MessageBox.Show($"Błąd podczas usuwania pojazdu: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}