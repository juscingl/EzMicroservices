namespace BuildingBlocks.Contracts.IntegrationEvents;

/// <summary>
/// 集成事件基类，定义跨服务消息的通用元数据。
/// </summary>
public abstract record IntegrationEvent
{
    /// <summary>
    /// 事件唯一标识，用于幂等与链路追踪。
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// 事件发生时间（UTC）。
    /// </summary>
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 关联标识，用于串联同一业务链路上的多个事件。
    /// </summary>
    public string? CorrelationId { get; init; }
}
