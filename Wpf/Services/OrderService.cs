using Wpf.Models.DTOs;

namespace Wpf.Services;

public class OrderService
{
    private readonly ApiClient _apiClient;
    private const string BaseEndpoint = "Orders";

    public OrderService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="command">The command containing data for the new order.</param>
    /// <returns>The created OrderDto or an error message.</returns>
    public async Task<(string? Result, string? ErrorMessage)> CreateOrderAsync(OrderDTO orderDto)
    {
        var command = new CreateOrderCommand
        (
            orderDto.IsPaid,
            orderDto.ClientId,
            orderDto.VehicleId,
            orderDto.ServicesToDo.ToList(),
            orderDto.SpareParts.ToList()
        );
        
        return await _apiClient.PostAsync<CreateOrderCommand, string>(BaseEndpoint, command);
    }

    /// <summary>
    /// Gets all orders without any filters.
    /// </summary>
    /// <returns>A list of OrderDto objects.</returns>
    public async Task<List<OrderDTO>?> GetOrdersAsync()
    {
        return await _apiClient.GetAsync<List<OrderDTO>>(BaseEndpoint);
    }

    /// <summary>
    /// Gets a single order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>The OrderDto object if found, otherwise null.</returns>
    public async Task<OrderDTO?> GetOrderByIdAsync(string id)
    {
        return await _apiClient.GetAsync<OrderDTO>($"{BaseEndpoint}/{id}");
    }

    /// <summary>
    /// Gets orders with optional filters.
    /// </summary>
    /// <param name="clientId">Optional: Filter by client ID (GUID string).</param>
    /// <param name="vehicleId">Optional: Filter by vehicle ID (GUID string).</param>
    /// <param name="isPaid">Optional: Filter by payment status.</param>
    /// <param name="finalizationDate">Optional: Filter orders finalized after this date.</param>
    /// <returns>A list of OrderDto objects matching the filters.</returns>
    public async Task<List<OrderDTO>?> GetOrdersWithFiltersAsync(
        string? clientId = null,
        string? vehicleId = null,
        bool? isPaid = null,
        DateOnly? finalizationDate = null)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            queryParams.Add($"clientId={Uri.EscapeDataString(clientId)}");
        }

        if (!string.IsNullOrWhiteSpace(vehicleId))
        {
            queryParams.Add($"vehicleId={Uri.EscapeDataString(vehicleId)}");
        }

        if (isPaid.HasValue)
        {
            queryParams.Add($"isPaid={isPaid.Value.ToString().ToLowerInvariant()}");
        }

        if (finalizationDate.HasValue)
        {
            queryParams.Add($"finalizationDate={finalizationDate.Value.ToString("yyyy-MM-dd")}");
        }

        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
        var endpoint = $"{BaseEndpoint}{queryString}";

        return await _apiClient.GetAsync<List<OrderDTO>>(endpoint);
    }
}