namespace BuildingBlocks.Auditing;

/// <summary>
/// 创建审计对象，要求同时具备创建时间与创建人信息。
/// </summary>
public interface ICreationAuditedObject : IHasCreationTime, IMayHaveCreator
{
}
