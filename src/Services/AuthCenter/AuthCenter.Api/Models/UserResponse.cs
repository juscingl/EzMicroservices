using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Models;

public sealed record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> DirectPermissions,
    IReadOnlyCollection<MenuNodeResponse> Menus);
