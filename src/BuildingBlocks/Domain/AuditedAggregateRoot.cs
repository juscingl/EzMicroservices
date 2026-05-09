using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

/// <summary>
/// 带完整修改审计信息的聚合根基类。
/// </summary>
public abstract class AuditedAggregateRoot : CreationAuditedAggregateRoot, IAuditedObject
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
