namespace Domain.Common;

public abstract class Entity : AuditableData
{ 
    public Guid Id { get; private init; }
    //We could create list of domain events for logging.
}