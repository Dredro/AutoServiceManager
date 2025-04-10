using Domain.Common;

namespace Domain.Entities;

public class OrderSparePart : Entity
{
    public Guid OrderId { get; set; }
    public required Order Order { get; set; }
    public Guid ProductId { get; set; }
    public required SparePart SparePart { get; set; }
    public int Quantity { get; set; }
}