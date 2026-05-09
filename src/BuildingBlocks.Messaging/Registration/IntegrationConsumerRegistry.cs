namespace BuildingBlocks.Messaging.Registration;

/// <summary>
/// 消费者注册表，集中维护当前服务声明的全部事件消费者。
/// </summary>
public sealed class IntegrationConsumerRegistry
{
    private readonly List<IntegrationConsumerRegistration> _registrations = [];

    /// <summary>
    /// 已注册的消费者集合（只读）。
    /// </summary>
    public IReadOnlyCollection<IntegrationConsumerRegistration> Registrations => _registrations.AsReadOnly();

    internal void Add(IntegrationConsumerRegistration registration)
    {
        _registrations.Add(registration);
    }
}
