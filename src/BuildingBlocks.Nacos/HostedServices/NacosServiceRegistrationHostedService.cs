using System.Net;
using System.Net.Sockets;
using BuildingBlocks.Nacos.Options;
using BuildingBlocks.Nacos.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Nacos.HostedServices;

internal sealed class NacosServiceRegistrationHostedService(
    INacosOpenApiClient nacosOpenApiClient,
    IOptions<NacosOptions> options,
    ILogger<NacosServiceRegistrationHostedService> logger) : BackgroundService
{
    private readonly NacosOptions _options = options.Value;
    private string? _registeredIp;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled || !_options.RegisterService)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.ServiceName) || _options.InstancePort <= 0)
        {
            logger.LogInformation("Nacos registration skipped because service name or instance port is not configured.");
            return;
        }

        _registeredIp = ResolveInstanceIp(_options.InstanceIp);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await nacosOpenApiClient.RegisterInstanceAsync(_registeredIp, _options.InstancePort, stoppingToken);
                logger.LogInformation(
                    "Registered service {ServiceName} to Nacos at {ServerAddress} with endpoint {Ip}:{Port}.",
                    _options.ServiceName,
                    _options.ServerAddress,
                    _registeredIp,
                    _options.InstancePort);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Nacos registration for service {ServiceName} failed. Retrying in 10 seconds.",
                    _options.ServiceName);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_options.Enabled && _options.RegisterService && !string.IsNullOrWhiteSpace(_registeredIp))
        {
            try
            {
                await nacosOpenApiClient.DeregisterInstanceAsync(_registeredIp, _options.InstancePort, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to deregister service {ServiceName} from Nacos.", _options.ServiceName);
            }
        }

        await base.StopAsync(cancellationToken);
    }

    private static string ResolveInstanceIp(string? configuredIp)
    {
        if (!string.IsNullOrWhiteSpace(configuredIp))
        {
            return configuredIp;
        }

        var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        var address = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        return address?.ToString() ?? IPAddress.Loopback.ToString();
    }
}
