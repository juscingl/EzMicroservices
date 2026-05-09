namespace BuildingBlocks.Auditing;

/// <summary>
/// 创建时间接口。
/// </summary>
public interface IHasCreationTime
{
    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    DateTimeOffset CreationTime { get; set; }
}
