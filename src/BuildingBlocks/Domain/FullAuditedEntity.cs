using BuildingBlocks.Auditing;

namespace BuildingBlocks.Domain;

/// <summary>
/// 带软删除信息的完整审计实体基类。
/// </summary>
public abstract class FullAuditedEntity : AuditedEntity, IFullAuditedObject
{
    /// <summary>
    /// 软删除标记。true 表示逻辑删除，不代表物理删除。
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间（UTC）。仅在软删除后有值。
    /// </summary>
    public DateTimeOffset? DeletionTime { get; set; }

    /// <summary>
    /// 删除人标识。
    /// </summary>
    public Guid? DeleterId { get; set; }
}
