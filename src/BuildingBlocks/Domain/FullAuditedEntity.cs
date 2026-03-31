using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

public abstract class FullAuditedEntity : AuditedEntity, IFullAuditedObject
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletionTime { get; set; }

    public Guid? DeleterId { get; set; }
}
