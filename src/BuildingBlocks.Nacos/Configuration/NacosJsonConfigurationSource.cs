using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Nacos.Configuration;

public sealed class NacosJsonConfigurationSource(NacosOptions options) : IConfigurationSource
{
    public NacosOptions Options { get; } = options;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new NacosJsonConfigurationProvider(Options);
    }
}
