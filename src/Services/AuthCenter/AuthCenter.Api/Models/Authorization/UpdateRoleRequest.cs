namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 更新角色基础信息请求模型。
/// </summary>
public sealed record UpdateRoleRequest(
    string Name,
    string Code,
    string? Description,
    int Sort,
    bool IsEnabled);
