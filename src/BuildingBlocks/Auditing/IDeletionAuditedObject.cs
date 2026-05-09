namespace BuildingBlocks.Auditing;

/// <summary>
/// 删除审计对象，记录删除时间和删除人。
/// </summary>
public interface IDeletionAuditedObject : ISoftDelete
{
    /// <summary>
    /// 删除时间（UTC）。
    /// </summary>
    DateTimeOffset? DeletionTime { get; set; }

    /// <summary>
    /// 删除人标识。
    /// </summary>
    Guid? DeleterId { get; set; }
}
