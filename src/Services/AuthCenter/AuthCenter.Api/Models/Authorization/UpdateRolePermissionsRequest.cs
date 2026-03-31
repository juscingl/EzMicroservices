namespace AuthCenter.Api.Models.Authorization;

public sealed record UpdateRolePermissionsRequest(
    IReadOnlyCollection<string> PermissionCodes);
