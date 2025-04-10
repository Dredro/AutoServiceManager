using Domain.Common;

namespace Domain.Entities;

public class Notification : Entity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool IsViewed { get; set; } 
    public required Worker Sender { get; set; }
    public ICollection<Worker> Receivers { get; set; } = [];
}