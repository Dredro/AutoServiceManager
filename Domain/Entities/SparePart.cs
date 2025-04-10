using Domain.Common;

namespace Domain.Entities;

public class SparePart : Entity
{
    public required string CatalogNumber { get; set; }
    public required string Name { get; set; }
    public char Quality { get; set; }
    public int QuantityInStock { get; set; }
    public ICollection<OrderSparePart> PartsAssignedToOrder { get; set; } = [];
}