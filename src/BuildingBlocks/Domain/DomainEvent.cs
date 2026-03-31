namespace BuildingBlocks.Domain;

public abstract record DomainEvent(DateTimeOffset OccurredOn);
