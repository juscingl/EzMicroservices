namespace BuildingBlocks.Domain;

/// <summary>
/// 实体标识接口，约束实体必须暴露主键。
/// </summary>
public interface IEntity<out TKey>
{
    /// <summary>
    /// 实体主键。
    /// </summary>
    TKey Id { get; }
}
