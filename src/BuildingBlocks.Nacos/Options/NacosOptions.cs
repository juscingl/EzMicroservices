namespace BuildingBlocks.Nacos.Options;

/// <summary>
/// Nacos 接入配置项。
/// </summary>
public sealed class NacosOptions
{
    /// <summary>
    /// 配置节名称。
    /// </summary>
    public const string SectionName = "Nacos";

    /// <summary>
    /// 是否启用 Nacos 组件。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否启用远程配置加载。
    /// </summary>
    public bool LoadConfiguration { get; set; } = true;

    /// <summary>
    /// 是否启用服务注册。
    /// </summary>
    public bool RegisterService { get; set; } = true;

    /// <summary>
    /// Nacos 服务器地址。
    /// </summary>
    public string ServerAddress { get; set; } = "http://localhost:8848";

    /// <summary>
    /// Nacos 命名空间。
    /// </summary>
    public string NamespaceId { get; set; } = "public";

    /// <summary>
    /// 配置/服务分组。
    /// </summary>
    public string Group { get; set; } = "DEFAULT_GROUP";

    /// <summary>
    /// 集群名称。
    /// </summary>
    public string ClusterName { get; set; } = "DEFAULT";

    /// <summary>
    /// 当前服务在 Nacos 中的服务名。
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// 远程配置 DataId。
    /// </summary>
    public string? ConfigDataId { get; set; }

    /// <summary>
    /// Nacos 登录用户名。
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Nacos 登录密码。
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 指定注册 IP；为空时自动探测。
    /// </summary>
    public string? InstanceIp { get; set; }

    /// <summary>
    /// 注册实例端口。
    /// </summary>
    public int InstancePort { get; set; }

    /// <summary>
    /// 实例元数据。
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// HTTP 请求超时时间（秒）。
    /// </summary>
    public int RequestTimeoutSeconds { get; set; } = 10;
}
