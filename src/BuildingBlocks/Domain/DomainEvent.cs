namespace BuildingBlocks.Domain;

/// <summary>
/// 领域事件基类型，记录事件发生时间。
/// </summary>
public abstract record DomainEvent(DateTimeOffset OccurredOn);
