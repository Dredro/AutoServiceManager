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

    public async Task<(ServiceDTO? Result, string? ErrorMessage)> CreateServiceAsync(CreateServiceCommand command)
    {
        return await _apiClient.PostAsync<CreateServiceCommand, ServiceDTO>(BaseEndpoint, command);
    }

    public async Task<List<ServiceDTO>?> GetServicesAsync()
    {
        return await _apiClient.GetAsync<List<ServiceDTO>>(BaseEndpoint);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateServiceAsync(int id, EditServiceCommand command)
    {
        return await _apiClient.PutAsync($"{BaseEndpoint}/{id}", command);
    }

    public async Task<ServiceDTO?> GetServiceByIdAsync(int id)
    {
        return await _apiClient.GetAsync<ServiceDTO>($"{BaseEndpoint}/{id}");
    }
    
}