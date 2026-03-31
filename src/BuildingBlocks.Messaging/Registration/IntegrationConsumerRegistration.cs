namespace BuildingBlocks.Messaging.Registration;

public sealed record IntegrationConsumerRegistration(
    Type EventType,
    Type HandlerType,
    string QueueName,
    string RoutingKey);
