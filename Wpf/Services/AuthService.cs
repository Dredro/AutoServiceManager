using Wpf.Models.DTOs;

namespace Wpf.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;
    public static string? CurrentToken { get; private set; } 
    public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentToken);

    public event Action? AuthenticationStateChanged;

    public AuthService(ApiClient apiClient)
    {
        _apiClient = apiClient;
        if(!string.IsNullOrEmpty(_apiClient.CurrentJwtToken))
        {
            SetToken(_apiClient.CurrentJwtToken);
        }
    }

    private void SetToken(string? token)
    {
        CurrentToken = token;
        _apiClient.SetJwtToken(token); 
        AuthenticationStateChanged?.Invoke();
    }

    public async Task<(bool Success, string? ErrorMessage, AccountResultDTO? Account)> LoginAsync(LoginDTO loginDto)
    {
        var (response, error) = await _apiClient.PostAsync<LoginDTO, AccountResultDTO>("auth/login", loginDto);
        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            SetToken(response.Token);
            return (true, null, response);
        }
        return (false, error ?? "Login failed. Check credentials or server response.", null);
    }

    public async Task<(bool Success, string? ErrorMessage, AccountResultDTO? Account)> RegisterAsync(CreateAccountDTO createAccountDto)
    {
        var (response, error) = await _apiClient.PostAsync<CreateAccountDTO, AccountResultDTO>("auth/register", createAccountDto);
        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            return (true, null, response);
        }
        return (false, error ?? "Registration failed.", null);
    }

    public void Logout()
    {
        SetToken(null);
    }
}