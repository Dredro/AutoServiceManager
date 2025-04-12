using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class SparePart : Entity
{
    public required string CatalogNumber { get; set; }
    public required string Name { get; set; }
    public required string Make { get; set; }
    public char Quality { get; set; }
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public required PartCategory Category { get; set; }
    public ICollection<OrderSparePart> PartsAssignedToOrder { get; set; } = [];
}