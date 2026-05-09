namespace BuildingBlocks.Auditing;

/// <summary>
/// 完整审计对象，除创建/修改外还包含删除审计信息。
/// </summary>
public interface IFullAuditedObject : IAuditedObject, IDeletionAuditedObject
{
}
