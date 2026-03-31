namespace BuildingBlocks.Nacos.Services;

public interface INacosOpenApiClient
{
    Task<string?> GetConfigAsync(CancellationToken cancellationToken = default);

    Task RegisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default);

    Task DeregisterInstanceAsync(string ip, int port, CancellationToken cancellationToken = default);
}
