namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 创建角色请求模型。
/// </summary>
public sealed record CreateRoleRequest(
    string Name,
    string Code,
    string? Description,
    int Sort,
    bool IsEnabled,
    IReadOnlyCollection<string> PermissionCodes);
