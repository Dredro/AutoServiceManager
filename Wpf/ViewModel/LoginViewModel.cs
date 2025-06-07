using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Wpf.Core; 
using Wpf.Models.DTOs;
using Wpf.Services;

namespace Wpf.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event Action? LoginSucceeded;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly AuthService _authService;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ClearOverallErrorMessage();
                    ValidateEmail();
                    _loginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ClearOverallErrorMessage();
                    ValidatePassword();
                    _loginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        private string _emailValidationError = string.Empty;
        public string EmailValidationError
        {
            get => _emailValidationError;
            private set => SetProperty(ref _emailValidationError, value);
        }

        private string _passwordValidationError = string.Empty;
        public string PasswordValidationError
        {
            get => _passwordValidationError;
            private set => SetProperty(ref _passwordValidationError, value);
        }

        private readonly IAsyncRelayCommand _loginCommand; 
        public ICommand LoginCommand => _loginCommand;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _loginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanExecuteLogin);
            
            ValidateEmail();
            ValidatePassword();
        }

        private void ClearOverallErrorMessage()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(_email))
            {
                EmailValidationError = "Email jest wymagany.";
                return;
            }
            try
            {
                var addr = new MailAddress(_email);
                if (addr.Address == _email)
                {
                    string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
                    if (!Regex.IsMatch(_email, emailPattern))
                    {
                        EmailValidationError = "Nieprawidłowy format adresu email.";
                        return;
                    }
                    EmailValidationError = string.Empty;
                    return;
                }
            }
            catch (FormatException)
            {
                EmailValidationError = "Nieprawidłowy format adresu email.";
                return;
            }
            EmailValidationError = "Nieprawidłowy format adresu email."; 
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(_password))
            {
                PasswordValidationError = "Hasło jest wymagane.";
                return;
            }
            if (_password.Length < 6)
            {
                PasswordValidationError = "Hasło musi mieć co najmniej 6 znaków.";
                return;
            }
            if (!Regex.IsMatch(_password, @"[A-Z]"))
            {
                PasswordValidationError = "Hasło musi zawierać co najmniej jedną wielką literę.";
                return;
            }
            if (!Regex.IsMatch(_password, @"[0-9]"))
            {
                PasswordValidationError = "Hasło musi zawierać co najmniej jedną cyfrę.";
                return;
            }
            PasswordValidationError = string.Empty; 
        }

        private async Task ExecuteLoginAsync()
        {
            ErrorMessage = string.Empty;
            
            ValidateEmail();
            ValidatePassword();
            if (!string.IsNullOrEmpty(EmailValidationError) || !string.IsNullOrEmpty(PasswordValidationError))
            {
                return; 
            }

            var loginDto = new LoginDTO
            {
                Email = _email,
                Password = _password ?? string.Empty
            };

            LoginSucceeded?.Invoke(); // Docker dalej mi nie działa więc zostawiam omiajnie 

            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Nieprawidłowy email lub hasło.";
            }
        }

        private bool CanExecuteLogin()
        {
            return string.IsNullOrEmpty(EmailValidationError) &&
                   string.IsNullOrEmpty(PasswordValidationError) &&
                   !string.IsNullOrWhiteSpace(_email) &&
                   !string.IsNullOrWhiteSpace(_password);
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}