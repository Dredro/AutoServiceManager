using Wpf.Models.DTOs;

namespace Wpf.Services;

public class VehicleService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "Vehicles";

    public VehicleService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(string? Result, string? ErrorMessage)> CreateVehicleAsync(CreateVehicleCommand command)
    {
        return await _apiClient.PostAsync<CreateVehicleCommand, string>(BaseEndpoint, command);
    }

    public async Task<List<VehicleDTO>?> GetVehiclesAsync()
    {
        return await _apiClient.GetAsync<List<VehicleDTO>>(BaseEndpoint);
    }

    public async Task<VehicleDTO?> GetVehicleByIdAsync(string vehicleId)
    {
        return await _apiClient.GetAsync<VehicleDTO>($"{BaseEndpoint}/{vehicleId}");
    }
    public async Task<(string? Result, string? ErrorMessage)> EditVehicleAsync(EditVehicleCommand command)
    {
        return await _apiClient.PatchAsync<EditVehicleCommand, string>(BaseEndpoint, command);
    }
    public async Task<(bool Success, string? ErrorMessage)> DeleteVehicleAsync(string id)
    {
        return await _apiClient.DeleteAsync($"{BaseEndpoint}/{id}");
    }
}