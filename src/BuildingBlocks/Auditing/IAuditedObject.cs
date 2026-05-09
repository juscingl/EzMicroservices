namespace BuildingBlocks.Auditing;

/// <summary>
/// 审计对象，包含创建与最后修改信息。
/// </summary>
public interface IAuditedObject : ICreationAuditedObject, IModificationAuditedObject
{
}
