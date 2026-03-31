namespace BuildingBlocks.Auditing;

public interface ICreationAuditedObject : IHasCreationTime, IMayHaveCreator
{
}
