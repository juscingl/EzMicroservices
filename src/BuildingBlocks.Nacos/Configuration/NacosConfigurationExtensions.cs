using BuildingBlocks.Nacos.Options;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Nacos.Configuration;

public static class NacosConfigurationExtensions
{
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
