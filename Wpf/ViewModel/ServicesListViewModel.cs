using System;
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
    public class ServicesListViewModel : INotifyPropertyChanged
    {
        private readonly ServiceService _serviceService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestCreateServiceView; 
        public event Action<Guid>? RequestEditServiceView; 

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            ((RelayCommand)LoadServicesCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand)CreateServiceCommand)?.RaiseCanExecuteChanged();
            ((RelayCommand<ServiceDTO>)EditServiceCommand)?.RaiseCanExecuteChanged(); 
            ((RelayCommand<ServiceDTO>)DeleteServiceCommand)?.RaiseCanExecuteChanged(); 
        }

        private ObservableCollection<ServiceDTO> _services = new ObservableCollection<ServiceDTO>();
        public ObservableCollection<ServiceDTO> Services
        {
            get => _services;
            set
            {
                if (_services != value)
                {
                    _services = value;
                    OnPropertyChanged(nameof(Services));
                }
            }
        }

        private ServiceDTO? _selectedService;
        public ServiceDTO? SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService != value)
                {
                    _selectedService = value;
                    OnPropertyChanged(nameof(SelectedService));
                    ((RelayCommand<ServiceDTO>)EditServiceCommand)?.RaiseCanExecuteChanged();
                    ((RelayCommand<ServiceDTO>)DeleteServiceCommand)?.RaiseCanExecuteChanged();
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
                }
            }
        }

        public ICommand LoadServicesCommand { get; private set; }
        public ICommand CreateServiceCommand { get; private set; }
        public ICommand EditServiceCommand { get; private set; }
        public ICommand DeleteServiceCommand { get; private set; }
        

        public ServicesListViewModel(ServiceService serviceService)
        {
            _serviceService = serviceService ?? throw new ArgumentNullException(nameof(serviceService));

            LoadServicesCommand = new RelayCommand(async () => await LoadServicesAsync(), () => !IsLoading);
            CreateServiceCommand = new RelayCommand(OnCreateService, () => !IsLoading);
            EditServiceCommand = new RelayCommand<ServiceDTO>(OnEditService, CanEditOrDeleteService);
            DeleteServiceCommand = new RelayCommand<ServiceDTO>(async (service) => await OnDeleteService(service), CanEditOrDeleteService);

            _ = LoadServicesAsync(); 
        }


        public async Task LoadServicesAsync()
        {
            IsLoading = true;
            try
            {
                var loadedServices = await _serviceService.GetServicesAsync();
                Application.Current.Dispatcher.Invoke(() => // Ensure UI update on UI thread
                {
                    Services.Clear();
                    foreach (var service in loadedServices.OrderBy(s => s.Name)) 
                    {
                        Services.Add(service);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania usług: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnCreateService()
        {
            RequestCreateServiceView?.Invoke(); 
        }

        private void OnEditService(ServiceDTO? serviceToEdit) 
        {
            if (serviceToEdit == null) return;
            RequestEditServiceView?.Invoke(serviceToEdit.Id); 
        }

        private async Task OnDeleteService(ServiceDTO? serviceToDelete) 
        {
            if (serviceToDelete == null) return;

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć usługę '{serviceToDelete.Name}'?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true; 
                try
                {
                    var (success, errorMessage) = await _serviceService.DeleteServiceAsync(serviceToDelete.Id.ToString());
                    if (success)
                    {
                        MessageBox.Show("Usługa została pomyślnie usunięta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                             Services.Remove(serviceToDelete); 
                             if (SelectedService == serviceToDelete) 
                             {
                                 SelectedService = null;
                             }
                        });
                    }
                    else
                    {
                        MessageBox.Show($"Błąd podczas usuwania usługi: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił nieoczekiwany błąd podczas usuwania: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
        private bool CanEditOrDeleteService(ServiceDTO? service)
        {
            return !IsLoading && service != null;
        }
    }
}