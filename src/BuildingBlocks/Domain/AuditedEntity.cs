using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

public abstract class AuditedEntity : CreationAuditedEntity, IAuditedObject
{
    public DateTimeOffset? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }
}
