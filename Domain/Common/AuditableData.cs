namespace Domain.Common;

public class AuditableData
{
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; set; }
}