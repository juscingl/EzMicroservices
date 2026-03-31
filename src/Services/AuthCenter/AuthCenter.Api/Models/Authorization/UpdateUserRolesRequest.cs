namespace AuthCenter.Api.Models.Authorization;

public sealed record UpdateUserRolesRequest(
    IReadOnlyCollection<string> Roles);
