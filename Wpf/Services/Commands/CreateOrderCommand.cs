using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateOrderCommand
{
    public Guid ClientId { get; set; }
    public Guid VehicleId { get; set; }
    public List<string> ServicesToDoIds { get; set; } = new List<string>();
    public List<OrderSparePartDTO> SpareParts { get; set; } = new List<OrderSparePartDTO>();
    public bool IsPaid { get; set; } = false;
}