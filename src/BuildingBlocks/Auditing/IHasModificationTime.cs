namespace BuildingBlocks.Auditing;

/// <summary>
/// 最后修改时间接口。
/// </summary>
public interface IHasModificationTime
{
    /// <summary>
    /// 最后修改时间（UTC）。
    /// </summary>
    DateTimeOffset? LastModificationTime { get; set; }
}
