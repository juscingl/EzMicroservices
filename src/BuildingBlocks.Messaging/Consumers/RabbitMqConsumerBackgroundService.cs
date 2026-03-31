using System.Reflection;
using BuildingBlocks.Messaging.Connection;
using BuildingBlocks.Messaging.Models;
using BuildingBlocks.Messaging.Options;
using BuildingBlocks.Messaging.Registration;
using BuildingBlocks.Messaging.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging.Consumers;

internal sealed class RabbitMqConsumerBackgroundService(
    IServiceProvider serviceProvider,
    RabbitMqConnectionProvider connectionProvider,
    IIntegrationEventSerializer serializer,
    IOptions<RabbitMqOptions> options,
    IntegrationConsumerRegistry consumerRegistry,
    ILogger<RabbitMqConsumerBackgroundService> logger) : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;
    private readonly List<IChannel> _channels = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled || consumerRegistry.Registrations.Count == 0)
        {
            logger.LogInformation("RabbitMQ consumer background service is idle.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var connection = await connectionProvider.GetConnectionAsync(stoppingToken);
                foreach (var registration in consumerRegistry.Registrations)
                {
                    var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                    _channels.Add(channel);

                    await ConfigureTopologyAsync(channel, registration, stoppingToken);
                    await StartConsumerAsync(channel, registration, stoppingToken);
                }

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RabbitMQ consumer initialization failed. Retrying in 5 seconds.");
                await DisposeChannelsAsync();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisposeChannelsAsync();
        await base.StopAsync(cancellationToken);
    }

    private async Task ConfigureTopologyAsync(
        IChannel channel,
        IntegrationConsumerRegistration registration,
        CancellationToken cancellationToken)
    {
        var deadLetterExchange = $"{registration.QueueName}.dlx";
        var deadLetterQueue = $"{registration.QueueName}.deadletter";
        var queueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = deadLetterExchange,
            ["x-dead-letter-routing-key"] = registration.QueueName
        };

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: deadLetterQueue,
            exchange: deadLetterExchange,
            routingKey: registration.QueueName,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: registration.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: registration.QueueName,
            exchange: _options.ExchangeName,
            routingKey: registration.RoutingKey,
            cancellationToken: cancellationToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: _options.PrefetchCount,
            global: false,
            cancellationToken: cancellationToken);
    }

    private async Task StartConsumerAsync(
        IChannel channel,
        IntegrationConsumerRegistration registration,
        CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var envelope = serializer.DeserializeEnvelope(args.Body);
                var payload = serializer.DeserializePayload(envelope, registration.EventType);
                var context = new IntegrationEventContext(
                    envelope.EventId,
                    envelope.CorrelationId,
                    envelope.OccurredOnUtc,
                    args.RoutingKey);

                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService(registration.HandlerType);
                await InvokeHandlerAsync(handler, registration.EventType, payload, context, cancellationToken);
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to process RabbitMQ message for queue {QueueName} and routing key {RoutingKey}.",
                    registration.QueueName,
                    registration.RoutingKey);
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: registration.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "RabbitMQ consumer started for queue {QueueName} and routing key {RoutingKey}.",
            registration.QueueName,
            registration.RoutingKey);
    }

    private static Task InvokeHandlerAsync(
        object handler,
        Type eventType,
        object payload,
        IntegrationEventContext context,
        CancellationToken cancellationToken)
    {
        var handlerInterface = typeof(BuildingBlocks.Messaging.Abstractions.IIntegrationEventHandler<>).MakeGenericType(eventType);
        var method = handlerInterface.GetMethod(nameof(BuildingBlocks.Messaging.Abstractions.IIntegrationEventHandler<BuildingBlocks.Contracts.IntegrationEvents.IntegrationEvent>.HandleAsync), BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"HandleAsync method not found on handler interface {handlerInterface.FullName}.");

        return (Task)(method.Invoke(handler, [payload, context, cancellationToken])
            ?? throw new InvalidOperationException($"Handler invocation failed for {handler.GetType().FullName}."));
    }

    private async Task DisposeChannelsAsync()
    {
        foreach (var channel in _channels)
        {
            await channel.DisposeAsync();
        }

        _channels.Clear();
    }
}
