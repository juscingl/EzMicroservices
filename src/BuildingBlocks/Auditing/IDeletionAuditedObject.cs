namespace BuildingBlocks.Auditing;

public interface IDeletionAuditedObject : ISoftDelete
{
    DateTimeOffset? DeletionTime { get; set; }

    Guid? DeleterId { get; set; }
}
