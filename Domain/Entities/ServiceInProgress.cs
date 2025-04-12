using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class ServiceInProgress : Entity
{
    public decimal? Price { get; set; }
    public DateTime? StartDate {get; set;}
    public DateTime? EndDate {get; set;}
    public required ServiceStatus ServiceStatus { get; set; }
    public required Service Service { get; set; }
    public required Order Order { get; set; }
    public ICollection<Worker> Workers { get; set; } = [];
}