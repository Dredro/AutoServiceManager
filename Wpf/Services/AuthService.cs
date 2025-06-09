using System.Text.Json;
using Wpf.Models;
using Wpf.Models.DTOs;

namespace Wpf.Services;

public class AuthService
{
    private readonly ApiClient _apiClient;
    public static string? CurrentToken { get; private set; } 
    public static Role? CurrentRole { get; private set; }
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
            var role = GetRoleFromEncodedJwt(response.Token);
            if (role != null)
            {
                CurrentRole = role switch
                {
                    "Mechanic" => Role.Mechanic,
                    "Manager" => Role.Manager,
                    "StorageManager" => Role.StorageManager,
                    _ => CurrentRole
                };
            }
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
    
    public static string? GetRoleFromTokenPayload(string jsonTokenPayload)
    {
        if (string.IsNullOrWhiteSpace(jsonTokenPayload))
        {
            return null; 
        }

        const string roleClaimKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        try
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonTokenPayload))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty(roleClaimKey, out JsonElement roleElement))
                {
                   
                    return roleElement.GetString();
                }
                else
                {
                    return null;
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Parsing error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public static string? GetRoleFromEncodedJwt(string encodedJwt)
    {
        if (string.IsNullOrWhiteSpace(encodedJwt))
        {
            return null;
        }

        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            
            if (!handler.CanReadToken(encodedJwt))
            {
                Console.WriteLine("String is not a valid JWT token.");
                return null;
            }

            var jwtToken = handler.ReadJwtToken(encodedJwt);
            
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);

            return roleClaim?.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while decoding jwt: {ex.Message}");
            return null;
        }
    }
}