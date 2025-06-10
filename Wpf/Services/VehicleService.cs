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

    public async Task<VehicleDTO?> GetVehicleByIdAsync(int vehicleId)
    {
        return await _apiClient.GetAsync<VehicleDTO>($"{BaseEndpoint}/{vehicleId}");
    }
}