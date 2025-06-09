using Wpf.Models.DTOs;

namespace Wpf.Services;

public class SparePartService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "SpareParts";

    public SparePartService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(SparePartsDTO? Result, string? ErrorMessage)> CreateSparePartAsync(CreateSparePartCommand command)
    {
        return await _apiClient.PostAsync<CreateSparePartCommand, SparePartsDTO>(BaseEndpoint, command);
    }

    public async Task<List<SparePartsDTO>?> GetSparePartsAsync()
    {
        return await _apiClient.GetAsync<List<SparePartsDTO>>(BaseEndpoint);
    }

    public async Task<SparePartsDTO?> GetSparePartByIdAsync(int id)
    {
        return await _apiClient.GetAsync<SparePartsDTO>($"{BaseEndpoint}/{id}");
    }
    
}