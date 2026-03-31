namespace AuthCenter.Api.Models.Authorization;

public sealed record CreateRoleRequest(
    string Name,
    IReadOnlyCollection<string> PermissionCodes);
