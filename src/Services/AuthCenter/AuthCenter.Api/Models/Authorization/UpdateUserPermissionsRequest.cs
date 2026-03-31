namespace AuthCenter.Api.Models.Authorization;

public sealed record UpdateUserPermissionsRequest(
    IReadOnlyCollection<string> PermissionCodes);
