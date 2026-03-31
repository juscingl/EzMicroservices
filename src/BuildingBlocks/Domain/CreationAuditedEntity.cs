using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

public abstract class CreationAuditedEntity : Entity, ICreationAuditedObject
{
    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }
}
