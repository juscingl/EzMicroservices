using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

public abstract class AuditedAggregateRoot : CreationAuditedAggregateRoot, IAuditedObject
{
    public DateTimeOffset? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }
}
