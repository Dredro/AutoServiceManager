using Wpf.Models.DTOs;

namespace Wpf.Services;

public class ServiceService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "Services";

    public ServiceService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(string? Result, string? ErrorMessage)> CreateServiceAsync(CreateServiceCommand command)
    {
        return await _apiClient.PostAsync<CreateServiceCommand, string>(BaseEndpoint, command);
    }

    public async Task<List<ServiceDTO>?> GetServicesAsync()
    {
        return await _apiClient.GetAsync<List<ServiceDTO>>(BaseEndpoint);
    }

    public async Task<(string? Success, string? ErrorMessage)> UpdateServiceAsync( EditServiceCommand command)
    {
        return await _apiClient.PatchAsync<EditServiceCommand, string>($"{BaseEndpoint}", command);
    }

    public async Task<ServiceDTO?> GetServiceByIdAsync(string id)
    {
        return await _apiClient.GetAsync<ServiceDTO>($"{BaseEndpoint}/{id}");
    }
    public async Task<(bool Success, string? ErrorMessage)> DeleteServiceAsync(string id)
    {
        return await _apiClient.DeleteAsync($"{BaseEndpoint}/{id}");
    }
}