namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 权限响应模型。
/// </summary>
public sealed record PermissionResponse(
    Guid Id,
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
    bool IsEnabled);
