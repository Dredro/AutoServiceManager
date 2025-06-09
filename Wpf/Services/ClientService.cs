using Wpf.Models.DTOs;

namespace Wpf.Services;

public class ClientService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "Clients";

    public ClientService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(ClientDTO? Result, string? ErrorMessage)> CreateClientAsync(CreateClientCommand command)
    {
        return await _apiClient.PostAsync<CreateClientCommand, ClientDTO>(BaseEndpoint, command);
    }

    public async Task<List<ClientDTO>?> GetClientsAsync()
    {
        return await _apiClient.GetAsync<List<ClientDTO>>(BaseEndpoint);
    }

    public async Task<ClientDTO?> GetClientByIdAsync(int id)
    {
        return await _apiClient.GetAsync<ClientDTO>($"{BaseEndpoint}/{id}");
    }
}