using Wpf.Models.DTOs;

namespace Wpf.Services;

public class WorkerService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "Workers";

    public WorkerService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<(WorkerDTO? Result, string? ErrorMessage)> CreateWorkerAsync(CreateWorkerCommand command)
    {
        return await _apiClient.PostAsync<CreateWorkerCommand, WorkerDTO>(BaseEndpoint, command);
    }

    public async Task<List<WorkerDTO>?> GetWorkersAsync()
    {
        return await _apiClient.GetAsync<List<WorkerDTO>>(BaseEndpoint);
    }

    public async Task<WorkerDTO?> GetWorkerByIdAsync(int id)
    {
        return await _apiClient.GetAsync<WorkerDTO>($"{BaseEndpoint}/{id}");
    }
    
}