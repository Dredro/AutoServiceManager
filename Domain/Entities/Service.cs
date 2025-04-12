using Domain.Common;

namespace Domain.Entities;

public class Service : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal MinimalPrice { get; set; }
    public decimal MaximalPrice { get; set; }
    public ICollection<ServiceInProgress> ServicesInProgress { get; set; }
}