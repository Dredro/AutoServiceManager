using Domain.Common;

namespace Domain.Entities;

public class Order : Entity
{
    public bool IsPaid { get; set; } 
    public DateOnly? FinalizationDate { get; set; }
    public required Client Client { get; set; }
    public Vehicle? Vehicle { get; set; }
    public ICollection<ServiceInProgress> ServicesToDo { get; set; } = [];
    public ICollection<OrderSparePart> SpareParts { get; set; } = [];
    
}