using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Nacos.Configuration;

/// <summary>
/// Nacos 配置接入扩展，用于在启动阶段按需加载远程配置。
/// </summary>
public static class NacosConfigurationExtensions
{
    /// <summary>
    /// 按配置开关将 Nacos JSON 配置源加入配置管道。
    /// </summary>
    public static IConfigurationBuilder AddNacosJsonConfiguration(
        this IConfigurationBuilder configurationBuilder,
        IConfiguration bootstrapConfiguration)
    {
        var options = bootstrapConfiguration
            .GetSection(NacosOptions.SectionName)
            .Get<NacosOptions>() ?? new NacosOptions();

        if (!options.Enabled || !options.LoadConfiguration || string.IsNullOrWhiteSpace(options.ConfigDataId))
        {
            return configurationBuilder;
        }

        configurationBuilder.Add(new NacosJsonConfigurationSource(options));
        return configurationBuilder;
    }
}
