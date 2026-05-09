using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

/// <summary>
/// 带完整修改审计信息的实体基类。
/// </summary>
public abstract class AuditedEntity : CreationAuditedEntity, IAuditedObject
{
    /// <summary>
    /// 最后修改时间（UTC）。
    /// </summary>
    public DateTimeOffset? LastModificationTime { get; set; }

    /// <summary>
    /// 最后修改人标识。
    /// </summary>
    public Guid? LastModifierId { get; set; }
}
