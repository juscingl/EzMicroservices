namespace BuildingBlocks.Domain;

/// <summary>
/// 领域实体基类，统一提供主键与领域事件容器。
/// </summary>
public abstract class Entity : IEntity<Guid>
{
    /// <summary>
    /// 实体主键。默认在实例化时生成，必要时可由持久化层覆盖。
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// 当前实体挂载的领域事件集合，只读暴露给外层处理器。
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// 清空已挂载的领域事件，通常在事件分发成功后调用。
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
