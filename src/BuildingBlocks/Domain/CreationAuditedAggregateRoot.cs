using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

public abstract class CreationAuditedAggregateRoot : AggregateRoot, ICreationAuditedObject
{
    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }
}
