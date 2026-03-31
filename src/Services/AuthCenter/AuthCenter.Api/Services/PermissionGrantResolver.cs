using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.EntityFrameworkCore;
using AuthCenter.Api.Identity;
using AuthCenter.Api.Models.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AuthCenter.Api.Services;

public sealed class PermissionGrantResolver(AuthCenterDbContext dbContext) : IPermissionGrantResolver
{
    public async Task<IReadOnlyCollection<string>> GetRolePermissionCodesAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var distinctRoleNames = roleNames
            .Where(roleName => !string.IsNullOrWhiteSpace(roleName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctRoleNames.Length == 0)
        {
            return Array.Empty<string>();
        }

        var roleIds = await dbContext.Set<ApplicationRole>()
            .AsNoTracking()
            .Where(role => role.Name != null && distinctRoleNames.Contains(role.Name))
            .Select(role => role.Id)
            .ToArrayAsync(cancellationToken);

        return await dbContext.RolePermissionGrants
            .AsNoTracking()
            .Where(grant => roleIds.Contains(grant.RoleId))
            .Join(
                dbContext.Permissions.AsNoTracking(),
                grant => grant.PermissionId,
                permission => permission.Id,
                (_, permission) => permission)
            .Where(permission => permission.IsEnabled)
            .Select(permission => permission.Code)
            .Distinct()
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetUserDirectPermissionCodesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.UserPermissionGrants
            .AsNoTracking()
            .Where(grant => grant.UserId == userId)
            .Join(
                dbContext.Permissions.AsNoTracking(),
                grant => grant.PermissionId,
                permission => permission.Id,
                (_, permission) => permission)
            .Where(permission => permission.IsEnabled)
            .Select(permission => permission.Code)
            .Distinct()
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetUserPermissionCodesAsync(
        Guid userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var permissions = new HashSet<string>(
            await GetRolePermissionCodesAsync(roleNames, cancellationToken),
            StringComparer.OrdinalIgnoreCase);

        foreach (var permission in await GetUserDirectPermissionCodesAsync(userId, cancellationToken))
        {
            permissions.Add(permission);
        }

        return permissions.ToArray();
    }

    public async Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(
        IEnumerable<string> permissionCodes,
        CancellationToken cancellationToken = default)
    {
        var distinctPermissionCodes = permissionCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctPermissionCodes.Length == 0)
        {
            return Array.Empty<MenuNodeResponse>();
        }

        var authorizedMenuIds = await dbContext.Permissions
            .AsNoTracking()
            .Where(permission => permission.IsEnabled
                && permission.MenuId.HasValue
                && distinctPermissionCodes.Contains(permission.Code))
            .Select(permission => permission.MenuId!.Value)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (authorizedMenuIds.Length == 0)
        {
            return Array.Empty<MenuNodeResponse>();
        }

        var menus = await dbContext.Menus
            .AsNoTracking()
            .Where(menu => menu.IsEnabled && menu.IsVisible)
            .OrderBy(menu => menu.Sort)
            .ThenBy(menu => menu.Name)
            .ToListAsync(cancellationToken);

        if (menus.Count == 0)
        {
            return Array.Empty<MenuNodeResponse>();
        }

        var menuById = menus.ToDictionary(menu => menu.Id);
        var includedMenuIds = new HashSet<Guid>(authorizedMenuIds);

        foreach (var menuId in authorizedMenuIds)
        {
            IncludeParents(menuId, menuById, includedMenuIds);
        }

        return BuildMenuTree(menus.Where(menu => includedMenuIds.Contains(menu.Id)).ToArray());
    }

    private static void IncludeParents(
        Guid menuId,
        IReadOnlyDictionary<Guid, PlatformMenu> menuById,
        ISet<Guid> includedMenuIds)
    {
        if (!menuById.TryGetValue(menuId, out var menu))
        {
            return;
        }

        var currentParentId = menu.ParentId;
        while (currentParentId.HasValue && menuById.TryGetValue(currentParentId.Value, out var parent))
        {
            includedMenuIds.Add(parent.Id);
            currentParentId = parent.ParentId;
        }
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
            .Select(menu => new MenuNodeResponse(
                menu.Id,
                menu.Code,
                menu.Name,
                menu.Route,
                menu.Icon,
                menu.Component,
                menu.Sort,
                menu.IsVisible,
                menu.IsEnabled,
                BuildNodes(menu.Id, childrenLookup)))
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
            .Select(menu => new MenuNodeResponse(
                menu.Id,
                menu.Code,
                menu.Name,
                menu.Route,
                menu.Icon,
                menu.Component,
                menu.Sort,
                menu.IsVisible,
                menu.IsEnabled,
                BuildNodes(menu.Id, childrenLookup)))
            .ToArray();
    }
}

