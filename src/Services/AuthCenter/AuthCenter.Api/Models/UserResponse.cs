using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Models;

/// <summary>
/// 用户信息响应模型。
/// </summary>
public sealed record UserResponse(
    Guid Id,
    string UserName,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    bool IsEnabled,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> DirectPermissions,
    IReadOnlyCollection<MenuNodeResponse> Menus);
