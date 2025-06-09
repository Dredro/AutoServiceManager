using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateOrderCommand
{
    public int ClientId { get; set; }
    public int VehicleId { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Description { get; set; }
    public List<OrderSparePartDTO>? SpareParts { get; set; } = new List<OrderSparePartDTO>();
    public List<int>? ServiceIds { get; set; } = new List<int>();
}