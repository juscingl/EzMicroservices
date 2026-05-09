namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 更新用户直授权限请求模型。
/// </summary>
public sealed record UpdateUserPermissionsRequest(
    IReadOnlyCollection<string> PermissionCodes);
