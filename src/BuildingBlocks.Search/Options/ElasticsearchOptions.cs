namespace BuildingBlocks.Search.Options;

/// <summary>
/// Elasticsearch 连接与索引命名配置。
/// </summary>
public sealed class ElasticsearchOptions
{
    /// <summary>
    /// 配置节名称。
    /// </summary>
    public const string SectionName = "Elasticsearch";

    /// <summary>
    /// 是否启用 Elasticsearch 相关能力。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Elasticsearch 服务地址。
    /// </summary>
    public string Uri { get; set; } = "http://localhost:9200";

    /// <summary>
    /// 默认索引前缀。
    /// </summary>
    public string DefaultIndexPrefix { get; set; } = "eztrade";

    /// <summary>
    /// Basic 认证用户名。
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Basic 认证密码。
    /// </summary>
    public string? Password { get; set; }
}
