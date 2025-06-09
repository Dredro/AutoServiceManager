using Application.Services.DTOs;
using Application.Workers.DTOs;
using Domain.Enums;

namespace Application.Orders.DTOs;

public class ServiceInProgressDto
{
    public Guid Id { get; set; }
    public decimal? Price { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ServiceStatus ServiceStatus { get; set; }
    public string ServiceId { get; set; }
    public OrderDto? Order { get; set; }
    public ICollection<GetWorkerDto> Workers { get; set; } = [];
}
