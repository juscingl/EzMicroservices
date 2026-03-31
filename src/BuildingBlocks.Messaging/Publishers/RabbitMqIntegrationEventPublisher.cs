using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Connection;
using BuildingBlocks.Messaging.Options;
using BuildingBlocks.Messaging.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Publishers;

internal sealed class RabbitMqIntegrationEventPublisher(
    RabbitMqConnectionProvider connectionProvider,
    IIntegrationEventSerializer serializer,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqIntegrationEventPublisher> logger) : IIntegrationEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options = options.Value;
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private IChannel? _channel;

    public async Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        if (!_options.Enabled)
        {
            logger.LogDebug("RabbitMQ is disabled. Event {EventType} was not published.", typeof(TEvent).Name);
            return;
        }

        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            var channel = await GetChannelAsync(cancellationToken);
            var body = serializer.SerializeEnvelope(integrationEvent);

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken);

            logger.LogInformation(
                "Published integration event {EventType} to exchange {ExchangeName} with routing key {RoutingKey}.",
                typeof(TEvent).Name,
                _options.ExchangeName,
                routingKey);
        }
        finally
        {
            _channelLock.Release();
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        var connection = await connectionProvider.GetConnectionAsync(cancellationToken);
        _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        _channelLock.Dispose();
    }
}
