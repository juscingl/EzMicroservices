namespace AuthCenter.Api.Models;

public sealed record CreateUserRequest(
    string UserName,
    string Email,
    string Password,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> DirectPermissionCodes);
