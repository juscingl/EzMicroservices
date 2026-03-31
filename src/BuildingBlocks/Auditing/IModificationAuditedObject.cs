namespace BuildingBlocks.Auditing;

public interface IModificationAuditedObject : IHasModificationTime
{
    Guid? LastModifierId { get; set; }
}
