namespace Domain.Common;

public abstract class Entity
{ 
    public Guid Id { get; private init; }
    protected Entity(Guid id)
    {
        Id = id;
    }
    protected Entity() { }
    //We could create list of domain events for logging.
}