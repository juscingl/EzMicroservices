namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 角色响应模型。
/// </summary>
public sealed record RoleResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    int Sort,
    bool IsEnabled,
    IReadOnlyCollection<string> PermissionCodes);
