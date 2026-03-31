using AuthCenter.Api.Models;
using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Services;

public interface IAuthorizationManagementService
{
    Task<CurrentUserProfileResponse?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetCurrentUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuNodeResponse>> GetCurrentUserMenusAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse?> UpdateUserRolesAsync(Guid userId, UpdateUserRolesRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse?> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    Task<RoleResponse?> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(CancellationToken cancellationToken = default);

    Task<MenuNodeResponse> SaveMenuAsync(Guid? menuId, SaveMenuRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionResponse>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    Task<PermissionResponse> SavePermissionAsync(Guid? permissionId, SavePermissionRequest request, CancellationToken cancellationToken = default);
}
