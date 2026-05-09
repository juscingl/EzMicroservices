using AuthCenter.Api.Models;
using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Services;

/// <summary>
/// 授权管理服务接口，封装用户/角色/菜单/权限的管理能力。
/// </summary>
public interface IAuthorizationManagementService
{
    /// <summary>
    /// 获取当前用户资料。
    /// </summary>
    Task<CurrentUserProfileResponse?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前用户权限编码集合。
    /// </summary>
    Task<IReadOnlyCollection<string>> GetCurrentUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前用户可见菜单树。
    /// </summary>
    Task<IReadOnlyCollection<MenuNodeResponse>> GetCurrentUserMenusAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse?> UpdateUserRolesAsync(Guid userId, UpdateUserRolesRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse?> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    Task<RoleResponse?> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default);

    Task<RoleResponse?> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(CancellationToken cancellationToken = default);

    Task<MenuNodeResponse> SaveMenuAsync(Guid? menuId, SaveMenuRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteMenuAsync(Guid menuId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionResponse>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    Task<PermissionResponse> SavePermissionAsync(Guid? permissionId, SavePermissionRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeletePermissionAsync(Guid permissionId, CancellationToken cancellationToken = default);
}
