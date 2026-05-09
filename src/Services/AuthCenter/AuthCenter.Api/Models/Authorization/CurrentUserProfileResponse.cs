namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 当前用户资料响应模型。
/// </summary>
public sealed record CurrentUserProfileResponse(
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
