namespace AuthCenter.Api.Models.Authorization;

public sealed record CurrentUserProfileResponse(
    Guid Id,
    string UserName,
    string Email,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> DirectPermissions,
    IReadOnlyCollection<MenuNodeResponse> Menus);
