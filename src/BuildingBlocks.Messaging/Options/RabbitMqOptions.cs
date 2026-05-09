using BuildingBlocks.Contracts.Messaging;

namespace BuildingBlocks.Messaging.Options;

/// <summary>
/// RabbitMQ 连接与消费行为配置。
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>
    /// 配置节名称。
    /// </summary>
    public const string SectionName = "RabbitMq";

    /// <summary>
    /// 是否启用 RabbitMQ。
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 服务地址。
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// 服务端口。
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// 登录用户名。
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// 登录密码。
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// 虚拟主机。
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// 事件发布交换机名称。
    /// </summary>
    public string ExchangeName { get; set; } = IntegrationExchangeNames.Platform;

    /// <summary>
    /// 单消费者预取消息数量。
    /// </summary>
    public ushort PrefetchCount { get; set; } = 16;

    /// <summary>
    /// RabbitMQ 客户端连接名，便于运维定位。
    /// </summary>
    public string ClientProvidedName { get; set; } = "eztrade-platform";

    /// <summary>
    /// 是否启用自动重连。
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>
    /// 自动重连间隔（秒）。
    /// </summary>
    public int NetworkRecoveryIntervalSeconds { get; set; } = 10;
}
