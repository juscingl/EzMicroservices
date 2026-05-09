using AuthCenter.Api.Authorization;
using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.EntityFrameworkCore;
using AuthCenter.Api.Identity;
using AuthCenter.Api.Models;
using AuthCenter.Api.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthCenter.Api.Services;

/// <summary>
/// 授权管理服务实现，负责用户、角色、菜单、权限的查询与维护。
/// </summary>
public sealed class AuthorizationManagementService(
    AuthCenterDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IPermissionGrantResolver permissionGrantResolver) : IAuthorizationManagementService
{
    private static readonly HashSet<string> AllowedPermissionTypes = new(
        PlatformPermissionConstants.PermissionTypes,
        StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<string> AllowedPermissionScopes = new(
        PlatformPermissionConstants.Scopes,
        StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 获取当前用户完整资料（角色、权限、菜单）。
    /// </summary>
    public async Task<CurrentUserProfileResponse?> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        var directPermissions = await permissionGrantResolver.GetUserDirectPermissionCodesAsync(user.Id, cancellationToken);
        var permissions = await permissionGrantResolver.GetUserPermissionCodesAsync(user.Id, roles, cancellationToken);
        var menus = await permissionGrantResolver.GetMenusAsync(permissions, cancellationToken);

        return new CurrentUserProfileResponse(
            user.Id,
            user.UserName ?? string.Empty,
            user.DisplayName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.IsEnabled,
            roles.ToArray(),
            permissions,
            directPermissions,
            menus);
    }

    /// <summary>
    /// 获取当前用户权限集合。
    /// </summary>
    public async Task<IReadOnlyCollection<string>> GetCurrentUserPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return Array.Empty<string>();
        }

        var roles = await userManager.GetRolesAsync(user);
        return await permissionGrantResolver.GetUserPermissionCodesAsync(user.Id, roles, cancellationToken);
    }

    /// <summary>
    /// 获取当前用户菜单树。
    /// </summary>
    public async Task<IReadOnlyCollection<MenuNodeResponse>> GetCurrentUserMenusAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetCurrentUserPermissionsAsync(userId, cancellationToken);
        return await permissionGrantResolver.GetMenusAsync(permissions, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var responses = new List<UserResponse>(users.Count);
        foreach (var user in users)
        {
            responses.Add(await MapUserResponseAsync(user, includeMenus: false, cancellationToken));
        }

        return responses;
    }

    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        return user is null
            ? null
            : await MapUserResponseAsync(user, includeMenus: true, cancellationToken);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            throw new InvalidOperationException("UserName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            throw new InvalidOperationException("DisplayName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("Email is required.");
        }

        await EnsureRolesExistAsync(request.Roles, cancellationToken);
        var permissions = await ResolvePermissionsByCodesAsync(request.DirectPermissionCodes, cancellationToken);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            DisplayName = request.DisplayName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsEnabled = request.IsEnabled,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        EnsureIdentityResult(createResult, "Failed to create user.");

        if (request.Roles.Count > 0)
        {
            var addRolesResult = await userManager.AddToRolesAsync(user, request.Roles);
            EnsureIdentityResult(addRolesResult, "Failed to assign roles.");
        }

        await ReplaceUserPermissionGrantsAsync(user.Id, permissions.Select(permission => permission.Id).ToArray(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapUserResponseAsync(user, includeMenus: true, cancellationToken);
    }

    public async Task<UserResponse?> UpdateUserRolesAsync(
        Guid userId,
        UpdateUserRolesRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureRolesExistAsync(request.Roles, cancellationToken);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return null;
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var desiredRoles = request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var rolesToRemove = currentRoles.Except(desiredRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (rolesToRemove.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            EnsureIdentityResult(removeResult, "Failed to remove existing roles.");
        }

        var rolesToAdd = desiredRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();
        if (rolesToAdd.Length > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
            EnsureIdentityResult(addResult, "Failed to assign roles.");
        }

        return await MapUserResponseAsync(user, includeMenus: true, cancellationToken);
    }

    public async Task<UserResponse?> UpdateUserPermissionsAsync(
        Guid userId,
        UpdateUserPermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return null;
        }

        var permissions = await ResolvePermissionsByCodesAsync(request.PermissionCodes, cancellationToken);
        await ReplaceUserPermissionGrantsAsync(user.Id, permissions.Select(permission => permission.Id).ToArray(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapUserResponseAsync(user, includeMenus: true, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;
        user.DeletionTime = DateTimeOffset.UtcNow;
        user.DeleterId = userId;
        await ReplaceUserPermissionGrantsAsync(user.Id, Array.Empty<Guid>(), cancellationToken);

        var existingRoles = await userManager.GetRolesAsync(user);
        if (existingRoles.Count > 0)
        {
            var removeRoleResult = await userManager.RemoveFromRolesAsync(user, existingRoles);
            EnsureIdentityResult(removeRoleResult, "Failed to remove user roles.");
        }

        var updateResult = await userManager.UpdateAsync(user);
        EnsureIdentityResult(updateResult, "Failed to delete user.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);

        var responses = new List<RoleResponse>(roles.Count);
        foreach (var role in roles)
        {
            responses.Add(await MapRoleResponseAsync(role, cancellationToken));
        }

        return responses;
    }

    public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Role name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Role code is required.");
        }

        var existingRole = await roleManager.FindByNameAsync(request.Name);
        if (existingRole is not null)
        {
            throw new InvalidOperationException($"Role '{request.Name}' already exists.");
        }

        var duplicateRoleCode = await roleManager.Roles
            .AsNoTracking()
            .AnyAsync(role => role.Code == request.Code, cancellationToken);
        if (duplicateRoleCode)
        {
            throw new InvalidOperationException($"Role code '{request.Code}' already exists.");
        }

        var permissions = await ResolvePermissionsByCodesAsync(request.PermissionCodes, cancellationToken);

        var role = new ApplicationRole(request.Name);
        role.Code = request.Code;
        role.Description = request.Description;
        role.Sort = request.Sort;
        role.IsEnabled = request.IsEnabled;
        var createResult = await roleManager.CreateAsync(role);
        EnsureIdentityResult(createResult, "Failed to create role.");

        await ReplaceRolePermissionGrantsAsync(role.Id, permissions.Select(permission => permission.Id).ToArray(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapRoleResponseAsync(role, cancellationToken);
    }

    public async Task<RoleResponse?> UpdateRoleAsync(
        Guid roleId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Role name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Role code is required.");
        }

        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
        {
            return null;
        }

        var duplicateName = await roleManager.Roles
            .AsNoTracking()
            .AnyAsync(item => item.Name == request.Name && item.Id != roleId, cancellationToken);
        if (duplicateName)
        {
            throw new InvalidOperationException($"Role '{request.Name}' already exists.");
        }

        var duplicateCode = await roleManager.Roles
            .AsNoTracking()
            .AnyAsync(item => item.Code == request.Code && item.Id != roleId, cancellationToken);
        if (duplicateCode)
        {
            throw new InvalidOperationException($"Role code '{request.Code}' already exists.");
        }

        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        role.Code = request.Code;
        role.Description = request.Description;
        role.Sort = request.Sort;
        role.IsEnabled = request.IsEnabled;
        var updateResult = await roleManager.UpdateAsync(role);
        EnsureIdentityResult(updateResult, "Failed to update role.");

        return await MapRoleResponseAsync(role, cancellationToken);
    }

    public async Task<RoleResponse?> UpdateRolePermissionsAsync(
        Guid roleId,
        UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var role = await roleManager.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == roleId, cancellationToken);

        if (role is null)
        {
            return null;
        }

        var permissions = await ResolvePermissionsByCodesAsync(request.PermissionCodes, cancellationToken);
        await ReplaceRolePermissionGrantsAsync(roleId, permissions.Select(permission => permission.Id).ToArray(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapRoleResponseAsync(role, cancellationToken);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role is null)
        {
            return false;
        }

        role.IsDeleted = true;
        role.DeletionTime = DateTimeOffset.UtcNow;
        role.DeleterId = roleId;
        await ReplaceRolePermissionGrantsAsync(roleId, Array.Empty<Guid>(), cancellationToken);

        var userRoleMappings = await dbContext.Set<IdentityUserRole<Guid>>()
            .Where(mapping => mapping.RoleId == roleId)
            .ToListAsync(cancellationToken);
        dbContext.RemoveRange(userRoleMappings);

        var updateResult = await roleManager.UpdateAsync(role);
        EnsureIdentityResult(updateResult, "Failed to delete role.");
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(CancellationToken cancellationToken = default)
    {
        var menus = await dbContext.Menus
            .AsNoTracking()
            .OrderBy(menu => menu.Sort)
            .ThenBy(menu => menu.Name)
            .ToListAsync(cancellationToken);

        return BuildMenuTree(menus);
    }

    public async Task<MenuNodeResponse> SaveMenuAsync(
        Guid? menuId,
        SaveMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Menu code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Menu name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Route))
        {
            throw new InvalidOperationException("Menu route is required.");
        }

        var trimmedLinkUrl = request.LinkUrl?.Trim();
        if (request.IsExternal && string.IsNullOrWhiteSpace(trimmedLinkUrl))
        {
            throw new InvalidOperationException("External menu must provide linkUrl.");
        }

        if (!request.IsExternal && !string.IsNullOrWhiteSpace(trimmedLinkUrl))
        {
            throw new InvalidOperationException("Internal menu cannot provide linkUrl.");
        }

        if (menuId.HasValue && request.ParentId == menuId)
        {
            throw new InvalidOperationException("A menu cannot reference itself as parent.");
        }

        if (request.ParentId.HasValue)
        {
            var parentExists = await dbContext.Menus
                .AnyAsync(menu => menu.Id == request.ParentId.Value, cancellationToken);
            if (!parentExists)
            {
                throw new InvalidOperationException("The specified parent menu does not exist.");
            }
        }

        var menu = menuId.HasValue
            ? await dbContext.Menus.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == menuId.Value, cancellationToken)
            : await dbContext.Menus.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Code == request.Code, cancellationToken);

        var duplicateMenu = await dbContext.Menus.IgnoreQueryFilters()
            .Where(item => item.Code == request.Code)
            .Where(item => !menuId.HasValue || item.Id != menuId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (duplicateMenu is not null && menu is null)
        {
            menu = duplicateMenu;
        }
        else if (duplicateMenu is not null)
        {
            throw new InvalidOperationException($"Menu code '{request.Code}' already exists.");
        }

        if (menu is null)
        {
            menu = new PlatformMenu();
            await dbContext.Menus.AddAsync(menu, cancellationToken);
        }

        menu.ParentId = request.ParentId;
        menu.Code = request.Code;
        menu.Name = request.Name;
        menu.Route = request.Route;
        menu.Icon = request.Icon;
        menu.Component = request.Component;
        menu.Sort = request.Sort;
        menu.IsVisible = request.IsVisible;
        menu.IsEnabled = request.IsEnabled;
        menu.IsExternal = request.IsExternal;
        menu.LinkUrl = request.IsExternal ? trimmedLinkUrl : null;
        menu.KeepAlive = request.KeepAlive;
        menu.HideInBreadcrumb = request.HideInBreadcrumb;
        menu.Description = request.Description;
        menu.IsDeleted = false;
        menu.DeletionTime = null;
        menu.DeleterId = null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapMenu(menu, Array.Empty<MenuNodeResponse>());
    }

    public async Task<bool> DeleteMenuAsync(Guid menuId, CancellationToken cancellationToken = default)
    {
        var menu = await dbContext.Menus.IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.Id == menuId, cancellationToken);
        if (menu is null)
        {
            return false;
        }

        var hasChildren = await dbContext.Menus.AnyAsync(item => item.ParentId == menuId, cancellationToken);
        if (hasChildren)
        {
            throw new InvalidOperationException("The menu has child nodes and cannot be deleted.");
        }

        var hasPermissions = await dbContext.Permissions.AnyAsync(item => item.MenuId == menuId, cancellationToken);
        if (hasPermissions)
        {
            throw new InvalidOperationException("The menu contains permissions and cannot be deleted.");
        }

        menu.IsDeleted = true;
        menu.DeletionTime = DateTimeOffset.UtcNow;
        menu.DeleterId = menuId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyCollection<PermissionResponse>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Resource)
            .ThenBy(permission => permission.Sort)
            .ThenBy(permission => permission.Name)
            .ToListAsync(cancellationToken);

        return permissions.Select(MapPermission).ToArray();
    }

    public async Task<PermissionResponse> SavePermissionAsync(
        Guid? permissionId,
        SavePermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Permission code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Permission name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Resource))
        {
            throw new InvalidOperationException("Permission resource is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Action))
        {
            throw new InvalidOperationException("Permission action is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PermissionType))
        {
            throw new InvalidOperationException("Permission type is required.");
        }

        if (!AllowedPermissionTypes.Contains(request.PermissionType))
        {
            throw new InvalidOperationException($"Permission type '{request.PermissionType}' is not supported.");
        }

        if (string.IsNullOrWhiteSpace(request.Scope))
        {
            throw new InvalidOperationException("Permission scope is required.");
        }

        if (!AllowedPermissionScopes.Contains(request.Scope))
        {
            throw new InvalidOperationException($"Permission scope '{request.Scope}' is not supported.");
        }

        if (string.IsNullOrWhiteSpace(request.GroupName))
        {
            throw new InvalidOperationException("Permission groupName is required.");
        }

        if (request.MenuId.HasValue)
        {
            var menuExists = await dbContext.Menus
                .AnyAsync(menu => menu.Id == request.MenuId.Value, cancellationToken);
            if (!menuExists)
            {
                throw new InvalidOperationException("The specified menu does not exist.");
            }
        }

        var permission = permissionId.HasValue
            ? await dbContext.Permissions.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Id == permissionId.Value, cancellationToken)
            : await dbContext.Permissions.IgnoreQueryFilters().FirstOrDefaultAsync(item => item.Code == request.Code, cancellationToken);

        var duplicatePermission = await dbContext.Permissions.IgnoreQueryFilters()
            .Where(item => item.Code == request.Code)
            .Where(item => !permissionId.HasValue || item.Id != permissionId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (duplicatePermission is not null && permission is null)
        {
            permission = duplicatePermission;
        }
        else if (duplicatePermission is not null)
        {
            throw new InvalidOperationException($"Permission code '{request.Code}' already exists.");
        }

        if (permission is null)
        {
            permission = new PlatformPermission();
            await dbContext.Permissions.AddAsync(permission, cancellationToken);
        }

        permission.MenuId = request.MenuId;
        permission.Code = request.Code;
        permission.Name = request.Name;
        permission.Resource = request.Resource;
        permission.Action = request.Action;
        permission.PermissionType = request.PermissionType;
        permission.Scope = request.Scope;
        permission.GroupName = request.GroupName.Trim();
        permission.Sort = request.Sort;
        permission.IsSystem = request.IsSystem;
        permission.IsEnabled = request.IsEnabled;
        permission.Description = request.Description;
        permission.IsDeleted = false;
        permission.DeletionTime = null;
        permission.DeleterId = null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapPermission(permission);
    }

    public async Task<bool> DeletePermissionAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        var permission = await dbContext.Permissions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.Id == permissionId, cancellationToken);
        if (permission is null)
        {
            return false;
        }

        var roleGrants = await dbContext.RolePermissionGrants
            .Where(item => item.PermissionId == permissionId)
            .ToListAsync(cancellationToken);
        if (roleGrants.Count > 0)
        {
            dbContext.RolePermissionGrants.RemoveRange(roleGrants);
        }

        var userGrants = await dbContext.UserPermissionGrants
            .Where(item => item.PermissionId == permissionId)
            .ToListAsync(cancellationToken);
        if (userGrants.Count > 0)
        {
            dbContext.UserPermissionGrants.RemoveRange(userGrants);
        }

        permission.IsDeleted = true;
        permission.DeletionTime = DateTimeOffset.UtcNow;
        permission.DeleterId = permissionId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<UserResponse> MapUserResponseAsync(
        ApplicationUser user,
        bool includeMenus,
        CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var directPermissions = await permissionGrantResolver.GetUserDirectPermissionCodesAsync(user.Id, cancellationToken);
        var permissions = await permissionGrantResolver.GetUserPermissionCodesAsync(user.Id, roles, cancellationToken);
        var menus = includeMenus
            ? await permissionGrantResolver.GetMenusAsync(permissions, cancellationToken)
            : Array.Empty<MenuNodeResponse>();

        return new UserResponse(
            user.Id,
            user.UserName ?? string.Empty,
            user.DisplayName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.IsEnabled,
            roles.ToArray(),
            permissions,
            directPermissions,
            menus);
    }

    private async Task<RoleResponse> MapRoleResponseAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        var permissions = await permissionGrantResolver.GetRolePermissionCodesAsync([role.Name ?? string.Empty], cancellationToken);
        return new RoleResponse(role.Id, role.Name ?? string.Empty, role.Code, role.Description, role.Sort, role.IsEnabled, permissions);
    }

    private async Task EnsureRolesExistAsync(IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        var distinctRoles = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctRoles.Length == 0)
        {
            return;
        }

        var existingRoles = await roleManager.Roles
            .AsNoTracking()
            .Where(role => role.Name != null && distinctRoles.Contains(role.Name))
            .Select(role => new { role.Name, role.IsEnabled })
            .ToArrayAsync(cancellationToken);

        var existingRoleNames = existingRoles
            .Select(role => role.Name!)
            .ToArray();
        var missingRoles = distinctRoles.Except(existingRoleNames, StringComparer.OrdinalIgnoreCase).ToArray();
        if (missingRoles.Length > 0)
        {
            throw new InvalidOperationException($"The following roles do not exist: {string.Join(", ", missingRoles)}.");
        }

        var disabledRoles = existingRoles
            .Where(role => !role.IsEnabled)
            .Select(role => role.Name!)
            .ToArray();
        if (disabledRoles.Length > 0)
        {
            throw new InvalidOperationException($"The following roles are disabled: {string.Join(", ", disabledRoles)}.");
        }
    }

    private async Task<IReadOnlyCollection<PlatformPermission>> ResolvePermissionsByCodesAsync(
        IEnumerable<string> permissionCodes,
        CancellationToken cancellationToken)
    {
        var distinctCodes = permissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctCodes.Length == 0)
        {
            return Array.Empty<PlatformPermission>();
        }

        var permissions = await dbContext.Permissions
            .Where(permission => distinctCodes.Contains(permission.Code) && permission.IsEnabled)
            .ToListAsync(cancellationToken);

        var missingCodes = distinctCodes
            .Except(permissions.Select(permission => permission.Code), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (missingCodes.Length > 0)
        {
            throw new InvalidOperationException($"The following permissions do not exist or are disabled: {string.Join(", ", missingCodes)}.");
        }

        return permissions;
    }

    private async Task ReplaceRolePermissionGrantsAsync(
        Guid roleId,
        IReadOnlyCollection<Guid> permissionIds,
        CancellationToken cancellationToken)
    {
        var existingGrants = await dbContext.RolePermissionGrants
            .Where(grant => grant.RoleId == roleId)
            .ToListAsync(cancellationToken);

        dbContext.RolePermissionGrants.RemoveRange(
            existingGrants.Where(grant => !permissionIds.Contains(grant.PermissionId)));

        var existingPermissionIds = existingGrants.Select(grant => grant.PermissionId).ToHashSet();
        foreach (var permissionId in permissionIds.Where(permissionId => !existingPermissionIds.Contains(permissionId)))
        {
            dbContext.RolePermissionGrants.Add(new PlatformRolePermissionGrant
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
        }
    }

    private async Task ReplaceUserPermissionGrantsAsync(
        Guid userId,
        IReadOnlyCollection<Guid> permissionIds,
        CancellationToken cancellationToken)
    {
        var existingGrants = await dbContext.UserPermissionGrants
            .Where(grant => grant.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.UserPermissionGrants.RemoveRange(
            existingGrants.Where(grant => !permissionIds.Contains(grant.PermissionId)));

        var existingPermissionIds = existingGrants.Select(grant => grant.PermissionId).ToHashSet();
        foreach (var permissionId in permissionIds.Where(permissionId => !existingPermissionIds.Contains(permissionId)))
        {
            dbContext.UserPermissionGrants.Add(new PlatformUserPermissionGrant
            {
                UserId = userId,
                PermissionId = permissionId
            });
        }
    }

    private static void EnsureIdentityResult(IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message} {errors}");
    }

    private static IReadOnlyCollection<MenuNodeResponse> BuildMenuTree(IReadOnlyCollection<PlatformMenu> menus)
    {
        var rootMenus = menus
            .Where(menu => !menu.ParentId.HasValue)
            .OrderBy(menu => menu.Sort)
            .ThenBy(menu => menu.Name)
            .ToArray();

        var childrenLookup = menus
            .Where(menu => menu.ParentId.HasValue)
            .GroupBy(menu => menu.ParentId!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(menu => menu.Sort).ThenBy(menu => menu.Name).ToArray());

        return rootMenus
            .Select(menu => MapMenu(menu, BuildNodes(menu.Id, childrenLookup)))
            .ToArray();
    }

    private static IReadOnlyCollection<MenuNodeResponse> BuildNodes(
        Guid parentId,
        IReadOnlyDictionary<Guid, PlatformMenu[]> childrenLookup)
    {
        if (!childrenLookup.TryGetValue(parentId, out var children))
        {
            return Array.Empty<MenuNodeResponse>();
        }

        return children
            .Select(menu => MapMenu(menu, BuildNodes(menu.Id, childrenLookup)))
            .ToArray();
    }

    private static MenuNodeResponse MapMenu(PlatformMenu menu, IReadOnlyCollection<MenuNodeResponse> children)
    {
        return new MenuNodeResponse(
            menu.Id,
            menu.Code,
            menu.Name,
            menu.Route,
            menu.Icon,
            menu.Component,
            menu.Sort,
            menu.IsVisible,
            menu.IsEnabled,
            menu.IsExternal,
            menu.LinkUrl,
            menu.KeepAlive,
            menu.HideInBreadcrumb,
            children);
    }

    private static PermissionResponse MapPermission(PlatformPermission permission)
    {
        return new PermissionResponse(
            permission.Id,
            permission.MenuId,
            permission.Code,
            permission.Name,
            permission.Resource,
            permission.Action,
            permission.PermissionType,
            permission.Scope,
            permission.GroupName,
            permission.Sort,
            permission.IsSystem,
            permission.IsEnabled);
    }
}

