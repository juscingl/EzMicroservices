namespace AuthCenter.Api.Models.Authorization;

public sealed record RoleResponse(
    Guid Id,
    string Name,
    IReadOnlyCollection<string> PermissionCodes);
