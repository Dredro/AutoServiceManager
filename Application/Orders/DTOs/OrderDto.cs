namespace Application.Orders.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string ClientId { get; set; }
    public string? VehicleId { get; set; }
    public bool IsPaid { get; set; }
    public DateOnly? FinalizationDate { get; set; }
    public List<ServiceInProgressDto> ServicesToDo { get; set; } = new List<ServiceInProgressDto>();
    public List<OrderSparePartDto> SpareParts { get; set; } = new List<OrderSparePartDto>();
}