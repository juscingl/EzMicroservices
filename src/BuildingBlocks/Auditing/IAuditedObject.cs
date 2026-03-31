namespace BuildingBlocks.Auditing;

public interface IAuditedObject : ICreationAuditedObject, IModificationAuditedObject
{
}
