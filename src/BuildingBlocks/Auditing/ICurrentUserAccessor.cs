namespace BuildingBlocks.Auditing;

/// <summary>
/// 当前用户访问器，用于在非 Controller 场景读取用户上下文。
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// 当前用户标识。未登录或系统调用场景可为空。
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// 当前用户名。未登录场景可为空。
    /// </summary>
    string? UserName { get; }
}
