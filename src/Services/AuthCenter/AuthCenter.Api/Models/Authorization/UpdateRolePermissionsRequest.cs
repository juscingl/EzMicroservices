namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 更新角色权限请求模型。
/// </summary>
public sealed record UpdateRolePermissionsRequest(
    IReadOnlyCollection<string> PermissionCodes);
