namespace AuthCenter.Api.Models;

/// <summary>
/// 创建用户请求模型。
/// </summary>
public sealed record CreateUserRequest(
    string UserName,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    string Password,
    bool IsEnabled,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> DirectPermissionCodes);
