using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Nacos.Configuration;

/// <summary>
/// Nacos JSON 配置源定义。
/// </summary>
public sealed class NacosJsonConfigurationSource(NacosOptions options) : IConfigurationSource
{
    /// <summary>
    /// 当前配置源使用的 Nacos 选项。
    /// </summary>
    public NacosOptions Options { get; } = options;

    /// <summary>
    /// 构建配置提供程序实例。
    /// </summary>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new NacosJsonConfigurationProvider(Options);
    }
}
