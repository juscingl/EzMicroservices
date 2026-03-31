namespace AuthCenter.Api.Models.Authorization;

public sealed record PermissionResponse(
    Guid Id,
    Guid? MenuId,
    string Code,
    string Name,
    string Resource,
    string Action,
    string PermissionType,
    int Sort,
    bool IsSystem,
    bool IsEnabled);
