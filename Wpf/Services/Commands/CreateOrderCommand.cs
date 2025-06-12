using Wpf.Models.DTOs;

namespace Wpf.Services;

public record CreateOrderCommand(
    bool IsPaid,
    string ClientId,
    string? VehicleId,
    List<ServiceInProgressDTO> ServicesToDo,
    List<OrderSparePartDTO> SpareParts
);