namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 更新用户角色请求模型。
/// </summary>
public sealed record UpdateUserRolesRequest(
    IReadOnlyCollection<string> Roles);
