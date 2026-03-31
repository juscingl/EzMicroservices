namespace AuthCenter.Api.Models.Authorization;

public sealed record SavePermissionRequest(
    Guid? MenuId,
    string Code,
    string Name,
    string Resource,
    string Action,
    string PermissionType,
    int Sort,
    bool IsSystem,
    bool IsEnabled,
    string? Description);
