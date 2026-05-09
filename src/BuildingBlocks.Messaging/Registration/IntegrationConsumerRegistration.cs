namespace BuildingBlocks.Messaging.Registration;

/// <summary>
/// 消费者注册描述，记录事件类型、处理器与队列绑定信息。
/// </summary>
public sealed record IntegrationConsumerRegistration(
    Type EventType,
    Type HandlerType,
    string QueueName,
    string RoutingKey);
