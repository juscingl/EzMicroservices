using BuildingBlocks.Contracts.Messaging;

namespace BuildingBlocks.Messaging.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public bool Enabled { get; set; } = true;

    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = "guest";

    public string Password { get; set; } = "guest";

    public string VirtualHost { get; set; } = "/";

    public string ExchangeName { get; set; } = IntegrationExchangeNames.Platform;

    public ushort PrefetchCount { get; set; } = 16;

    public string ClientProvidedName { get; set; } = "eztrade-platform";

    public bool AutomaticRecoveryEnabled { get; set; } = true;

    public int NetworkRecoveryIntervalSeconds { get; set; } = 10;
}
