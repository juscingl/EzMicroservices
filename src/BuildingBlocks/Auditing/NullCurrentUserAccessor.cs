namespace BuildingBlocks.Auditing;

/// <summary>
/// 空对象实现：在无用户上下文时返回默认值，避免调用方反复判空。
/// </summary>
public sealed class NullCurrentUserAccessor : ICurrentUserAccessor
{
    /// <summary>
    /// 单例实例。
    /// </summary>
    public static NullCurrentUserAccessor Instance { get; } = new();

    /// <summary>
    /// 无用户上下文时固定返回 null。
    /// </summary>
    public Guid? UserId => null;

    /// <summary>
    /// 无用户上下文时固定返回 null。
    /// </summary>
    public string? UserName => null;

    private NullCurrentUserAccessor()
    {
    }
}
