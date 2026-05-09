namespace BuildingBlocks.Auditing;

/// <summary>
/// 修改审计对象，记录最后修改人信息。
/// </summary>
public interface IModificationAuditedObject : IHasModificationTime
{
    /// <summary>
    /// 最后修改人标识。
    /// </summary>
    Guid? LastModifierId { get; set; }
}
