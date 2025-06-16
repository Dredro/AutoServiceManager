using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models; 
using Wpf.Services; 

namespace Wpf.ViewModels
{
    public class EditServiceFormViewModel : INotifyPropertyChanged
    {
        private readonly ServiceService _serviceService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? ServiceUpdated; 
        public event Action? RequestGoBack; 

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private Guid _serviceId;
        private string _serviceName = string.Empty;
        private string _serviceDescription = string.Empty;
        private decimal _minimalPrice;
        private decimal _maximalPrice;
        private bool _isSaving;
        private bool _isLoading; 

        public Guid ServiceId
        {
            get => _serviceId;
            private set
            {
                if (_serviceId != value)
                {
                    _serviceId = value;
                    OnPropertyChanged(nameof(ServiceId));
                }
            }
        }

        public string ServiceName
        {
            get => _serviceName;
            set
            {
                if (_serviceName != value)
                {
                    _serviceName = value;
                    OnPropertyChanged(nameof(ServiceName));
                }
            }
        }

        public string ServiceDescription
        {
            get => _serviceDescription;
            set
            {
                if (_serviceDescription != value)
                {
                    _serviceDescription = value;
                    OnPropertyChanged(nameof(ServiceDescription));
                }
            }
        }

        public decimal MinimalPrice
        {
            get => _minimalPrice;
            set
            {
                if (_minimalPrice != value)
                {
                    _minimalPrice = value;
                    OnPropertyChanged(nameof(MinimalPrice));
                }
            }
        }

        public decimal MaximalPrice
        {
            get => _maximalPrice;
            set
            {
                if (_maximalPrice != value)
                {
                    _maximalPrice = value;
                    OnPropertyChanged(nameof(MaximalPrice));
                }
            }
        }

        public bool IsSaving 
        {
            get => _isSaving || _isLoading; 
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    OnPropertyChanged(nameof(IsSaving));
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    OnPropertyChanged(nameof(IsSaving)); 
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }


        public EditServiceFormViewModel(ServiceService serviceService)
        {
            _serviceService = serviceService ?? throw new ArgumentNullException(nameof(serviceService));

            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }
        
        public async Task LoadServiceAsync(Guid serviceId)
        {
            IsLoading = true; 
            try
            {
                ServiceId = serviceId; 
               var service = await _serviceService.GetServiceByIdAsync(serviceId.ToString());

                if (service != null)
                {
                    ServiceName = service.Name;
                    ServiceDescription = service.Description;
                    MinimalPrice = service.MinimalPrice;
                    MaximalPrice = service.MaximalPrice;
                }
                else
                {
                    MessageBox.Show($"Błąd podczas ładowania usługi", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    RequestGoBack?.Invoke(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd podczas ładowania usługi: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                RequestGoBack?.Invoke(); 
            }
            finally
            {
                IsLoading = false; 
            }
        }

        private async Task OnSave()
        {
            if (IsSaving) return; // Prevent multiple saves

            IsSaving = true; // Set saving state
            try
            {
                // Validation handled by CanSave(), but you could add a user message here
                if (!CanSave())
                {
                    MessageBox.Show("Proszę wypełnić nazwę usługi oraz upewnić się, że zakres ceny jest prawidłowy (liczby nieujemne; 'Od' nie może być większe niż 'Do').", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var command = new EditServiceCommand() 
                {
                    Id = ServiceId.ToString(), 
                    Name = ServiceName,
                    Description = ServiceDescription,
                    MinimalPrice = MinimalPrice,
                    MaximalPrice = MaximalPrice
                };

                var (resultService, errorMessage) = await _serviceService.UpdateServiceAsync(command); // Call Update method

                if (errorMessage != null)
                {
                    MessageBox.Show("Usługa została pomyślnie zaktualizowana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ServiceUpdated?.Invoke(); 
                }
                else
                {
                    MessageBox.Show($"Błąd podczas aktualizacji usługi: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false; // Clear saving state
            }
        }

        private bool CanSave()
        {
            /*if (IsSaving) return false; 
            if (string.IsNullOrWhiteSpace(ServiceName)) return false;
            if (MinimalPrice < 0) return false;
            if (MaximalPrice < 0) return false;
            if (MinimalPrice > MaximalPrice) return false;*/

            return true;
        }

        private void OnCancel()
        {
            if (MessageBox.Show("Czy na pewno chcesz anulować edycję? Niezapisane zmiany zostaną utracone.", "Anuluj Edycję", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RequestGoBack?.Invoke(); 
            }
        }
    }
}