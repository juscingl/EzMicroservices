namespace BuildingBlocks.Nacos.Services;

/// <summary>
/// Nacos OpenAPI 客户端抽象，封装配置拉取与服务实例注册能力。
/// </summary>
public interface INacosOpenApiClient
{
    /// <summary>
    /// 从 Nacos 拉取配置内容。
    /// </summary>
    Task<string?> GetConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 向 Nacos 注册当前服务实例。
    /// </summary>
    Task RegisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从 Nacos 注销当前服务实例。
    /// </summary>
    Task DeregisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default);
}
