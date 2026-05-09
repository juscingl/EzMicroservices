namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 新增或更新权限请求模型。
/// </summary>
public sealed record SavePermissionRequest(
    Guid? MenuId,
    string Code,
    string Name,
    string Resource,
    string Action,
    string PermissionType,
    string Scope,
    string? GroupName,
    int Sort,
    bool IsSystem,
    bool IsEnabled,
    string? Description);
