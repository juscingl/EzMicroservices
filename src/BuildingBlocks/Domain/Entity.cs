namespace BuildingBlocks.Domain;

public abstract class Entity : IEntity<Guid>
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
