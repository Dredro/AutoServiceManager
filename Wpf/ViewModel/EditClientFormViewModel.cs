using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Services;

namespace Wpf.ViewModels
{
    public class EditClientFormViewModel : INotifyPropertyChanged
    {
        private readonly ClientService _clientService;
        private Guid _clientId; 

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? ClientUpdated; 
        public event Action? RequestGoBack; 

        // Bound Properties
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
                    ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditClientFormViewModel(ClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));

            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }

        /// <summary>
        /// Loads the client data into the form fields. This method should be called
        /// by the parent ViewModel (e.g., MainViewModel) after creating an instance.
        /// </summary>
        /// <param name="clientId">The GUID of the client to edit.</param>
        public async Task LoadClientAsync(Guid clientId)
        {
            _clientId = clientId;
            IsSaving = true; 
            try
            {
                var client = await _clientService.GetClientByIdAsync(clientId.ToString());
                if (client != null)
                {
                    FirstName = client.FirstName;
                    LastName = client.LastName;
                    Email = client.Email;
                    PhoneNumber = client.PhoneNumber;
                }
                else
                {
                    MessageBox.Show($"Nie znaleziono klienta o ID: {clientId}. Być może został usunięty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    RequestGoBack?.Invoke(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania danych klienta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                RequestGoBack?.Invoke(); 
            }
            finally
            {
                IsSaving = false; 
            }
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

                var command = new EditClientCommand()
                {
                    Id = _clientId.ToString(),
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber
                };

                var (result, errorMessage) = await _clientService.EditClientAsync(command);

                if (errorMessage == null)
                {
                    MessageBox.Show("Dane klienta zostały pomyślnie zaktualizowane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClientUpdated?.Invoke();
                }
                else
                {
                    MessageBox.Show($"Błąd podczas aktualizacji klienta: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd podczas aktualizacji: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSave()
        {
            /*// Simple validation: ensure essential fields are not empty and not currently saving
            return !IsSaving &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber); // Email might be optional based on business rules*/
            return true;
        }

        private void OnCancel()
        {
            if (MessageBox.Show("Czy na pewno chcesz anulować edycję? Niezapisane zmiany zostaną utracone.", "Anuluj Edycję", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RequestGoBack?.Invoke(); 
            }
        }

        private bool CanCancel()
        {
            return !IsSaving;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(FirstName) ||
                propertyName == nameof(LastName) ||
                propertyName == nameof(Email) ||
                propertyName == nameof(PhoneNumber))
            {
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }
}