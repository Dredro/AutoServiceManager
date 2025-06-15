using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;      
using System.Windows.Input;
using Wpf.Core;            
using Wpf.Services;        

namespace Wpf.ViewModels
{
    public class CreateServiceFormViewModel : INotifyPropertyChanged 
    {
        private readonly ServiceService _serviceService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? ServiceCreated; 
        public event Action? RequestGoBack;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); 
        }
        
        private string _serviceName = string.Empty;
        private string _serviceDescription = string.Empty; 
        private decimal _minimalPrice; 
        private decimal _maximalPrice; 
        private bool _isSaving;

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

        public ICommand SaveCommand { get; private set; } 
        public ICommand CancelCommand { get; private set; } 
        
        
        public CreateServiceFormViewModel(ServiceService serviceService)
        {
            _serviceService = serviceService ?? throw new ArgumentNullException(nameof(serviceService));

            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave); 
            CancelCommand = new RelayCommand(OnCancel);

            ClearForm(); 
        }

        private async Task OnSave()
        {
            if (IsSaving) return; 

            IsSaving = true; 
            try
            {
                /*if (!CanSave()) 
                {
                    MessageBox.Show("Proszę wypełnić nazwę usługi oraz upewnić się, że zakres ceny jest prawidłowy (liczby nieujemne; 'Od' nie może być większe niż 'Do').", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                */

                var command = new CreateServiceCommand
                {
                    Name = ServiceName,
                    Description = ServiceDescription, 
                    MinimalPrice = MinimalPrice, 
                    MaximalPrice = MaximalPrice  
                };

                var (resultService, errorMessage) = await _serviceService.CreateServiceAsync(command);

                if (resultService != null)
                {
                    MessageBox.Show("Usługa została pomyślnie dodana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm(); 
                    ServiceCreated?.Invoke(); 
                }
                else
                {
                    MessageBox.Show($"Błąd podczas dodawania usługi: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (IsSaving) return false; 
            if (string.IsNullOrWhiteSpace(ServiceName)) return false;
            if (MinimalPrice < 0) return false;
            if (MaximalPrice < 0) return false;
            if (MinimalPrice > MaximalPrice) return false;

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
            ServiceName = string.Empty;
            ServiceDescription = string.Empty;
            MinimalPrice = 0m; 
            MaximalPrice = 0m;   
        }
    }
}