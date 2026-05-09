namespace BuildingBlocks.Auditing;

/// <summary>
/// 软删除接口，标识数据是否被逻辑删除。
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// 软删除标记。true 表示逻辑删除。
    /// </summary>
    bool IsDeleted { get; set; }
}
