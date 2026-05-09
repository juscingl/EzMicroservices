namespace BuildingBlocks.Search.Abstractions;

/// <summary>
/// 索引名称解析器，用于将业务逻辑名转换为物理索引名。
/// </summary>
public interface IIndexNameResolver
{
    /// <summary>
    /// 解析索引名称。
    /// </summary>
    string Resolve(string logicalName);
}
