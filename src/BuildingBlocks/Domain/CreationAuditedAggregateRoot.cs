using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

/// <summary>
/// 带创建审计信息的聚合根基类。
/// </summary>
public abstract class CreationAuditedAggregateRoot : AggregateRoot, ICreationAuditedObject
{
    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人标识。匿名或系统任务场景可为空。
    /// </summary>
    public Guid? CreatorId { get; set; }
}
