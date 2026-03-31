using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Services;

public interface IPermissionGrantResolver
{
    Task<IReadOnlyCollection<string>> GetRolePermissionCodesAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetUserDirectPermissionCodesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetUserPermissionCodesAsync(
        Guid userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(
        IEnumerable<string> permissionCodes,
        CancellationToken cancellationToken = default);
}
