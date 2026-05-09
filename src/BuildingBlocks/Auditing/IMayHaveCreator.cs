namespace BuildingBlocks.Auditing;

/// <summary>
/// 可选创建人接口，用于记录创建人标识。
/// </summary>
public interface IMayHaveCreator
{
    /// <summary>
    /// 创建人标识。匿名或系统任务场景可为空。
    /// </summary>
    Guid? CreatorId { get; set; }
}
