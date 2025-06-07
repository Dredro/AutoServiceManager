using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;
using System.Windows;

namespace Wpf.ViewModel;


public class LoginViewModel : INotifyPropertyChanged
{
    public event Action? LoginSucceeded;
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand LoginCommand { get; }

    private readonly AuthService? _authService;

    #region FormData

    private string _email = String.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(_email); }
    }

    private string? _password; // Cannot declare a var because VM have no rights to access via PasswordBoxAssistant :(
    public string? Password
    {
        get => _password;
        set { _password = value;}
    }

    private string _errorMessage = String.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(_errorMessage); }
    }
    #endregion

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        LoginCommand = new AsyncRelayCommand(ExecuteLogin, CanExecuteLogin);
    }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(Login);
    }

    private async Task ExecuteLogin()
    {
        var loginVar = new LoginDTO();
        loginVar.Email = _email;
        loginVar.Password = (!string.IsNullOrWhiteSpace(_password) ? _password : String.Empty);

        ErrorMessage = string.Empty;

        if (_authService == null) return;
        var result = await _authService.LoginAsync(loginVar);

        if (!result.Success)
        {
            ErrorMessage = (result.ErrorMessage == null ? String.Empty : result.ErrorMessage);
            return;
        }

        Login();
    }

    private void Login()
    {
        MessageBox.Show($"pass: {Password}, Login: {Email}");

        bool authenticated = true;

        if (authenticated)
            LoginSucceeded?.Invoke();
    }


    private bool CanExecuteLogin() =>
        !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
