using System;
using System.ComponentModel;
using System.Windows; 
using System.Windows.Input;
using Wpf.Core; 
using Wpf.Services;


namespace Wpf.ViewModels
{
    public class CreateClientFormViewModel : INotifyPropertyChanged
    {
        private readonly ClientService _clientService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? ClientCreated; 
        public event Action? RequestGoBack;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _phoneNumber = string.Empty;
        private bool _isSaving;

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
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
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateClientFormViewModel(ClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave);
            CancelCommand = new RelayCommand(OnCancel);
            ClearForm();
        }

        private async Task OnSave()
        {
            IsSaving = true;
            try
            {
                if (!CanSave())
                {
                    MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var command = new CreateClientCommand
                {
                    Email = _email,
                    PhoneNumber = _phoneNumber,
                    FirstName = _firstName,
                    LastName = _lastName
                    
                };

                var (resultClient, errorMessage) = await _clientService.CreateClientAsync(command);

                if (resultClient != null)
                {
                    ClearForm();
                    ClientCreated?.Invoke(); 
                }
                else
                {
                    MessageBox.Show($"Błąd podczas dodawania klienta: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
            /*return !IsSaving &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber);*/
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
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}