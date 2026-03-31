using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.EntityFrameworkCore;
using AuthCenter.Api.Identity;
using AuthCenter.Api.Options;
using BuildingBlocks.Security.Constants;
using BuildingBlocks.Security.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthCenter.Api.Services;

public sealed class AuthCenterDataSeeder(
    AuthCenterDbContext dbContext,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager,
    ISeededClientRegistry seededClientRegistry,
    IOptions<AuthCenterSeedOptions> seedOptions,
    IOptions<PlatformAuthenticationOptions> authenticationOptions) : IAuthCenterDataSeeder
{
    private readonly AuthCenterSeedOptions _seedOptions = seedOptions.Value;
    private readonly PlatformAuthenticationOptions _authenticationOptions = authenticationOptions.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureScopesAsync(cancellationToken);
        await EnsureClientsAsync(cancellationToken);
        await EnsureMenusAsync(cancellationToken);
        await EnsurePermissionsAsync(cancellationToken);
        await EnsureRolesAsync();
        await EnsureRolePermissionGrantsAsync(cancellationToken);
        await EnsureAdminUserAsync();
    }

    private async Task EnsureScopesAsync(CancellationToken cancellationToken)
    {
        foreach (var scopeName in PlatformScopes.All)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await scopeManager.FindByNameAsync(scopeName) is not null)
            {
                continue;
            }

            var descriptor = new OpenIddictScopeDescriptor
            {
                Name = scopeName,
                DisplayName = $"{scopeName} API access"
            };

            descriptor.Resources.Add(_authenticationOptions.Audience);
            await scopeManager.CreateAsync(descriptor);
        }
    }

    private async Task EnsureClientsAsync(CancellationToken cancellationToken)
    {
        foreach (var client in seededClientRegistry.GetAll())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await applicationManager.FindByClientIdAsync(client.ClientId) is not null)
            {
                continue;
            }

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = client.ClientId,
                ClientType = client.ClientType,
                ConsentType = ConsentTypes.Implicit,
                DisplayName = client.DisplayName
            };

            if (!string.IsNullOrWhiteSpace(client.ClientSecret))
            {
                descriptor.ClientSecret = client.ClientSecret;
            }

            descriptor.Permissions.Add(Permissions.Endpoints.Token);
            descriptor.Permissions.Add(Permissions.Endpoints.Revocation);

            foreach (var grantType in client.GrantTypes)
            {
                descriptor.Permissions.Add(grantType switch
                {
                    GrantTypes.Password => Permissions.GrantTypes.Password,
                    GrantTypes.RefreshToken => Permissions.GrantTypes.RefreshToken,
                    GrantTypes.ClientCredentials => Permissions.GrantTypes.ClientCredentials,
                    _ => throw new InvalidOperationException($"Grant type '{grantType}' is not supported by the seed configuration.")
                });
            }

            foreach (var scope in client.AllowedScopes)
            {
                descriptor.Permissions.Add($"{Permissions.Prefixes.Scope}{scope}");
            }

            await applicationManager.CreateAsync(descriptor);
        }
    }

    private async Task EnsureMenusAsync(CancellationToken cancellationToken)
    {
        var existingMenus = await dbContext.Menus
            .IgnoreQueryFilters()
            .ToDictionaryAsync(menu => menu.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var seedMenu in PlatformAuthorizationSeedCatalog.Menus.OrderBy(menu => menu.Sort))
        {
            if (!existingMenus.TryGetValue(seedMenu.Code, out var menu))
            {
                menu = new PlatformMenu();
                await dbContext.Menus.AddAsync(menu, cancellationToken);
                existingMenus[seedMenu.Code] = menu;
            }

            menu.Code = seedMenu.Code;
            menu.Name = seedMenu.Name;
            menu.Route = seedMenu.Route;
            menu.Icon = seedMenu.Icon;
            menu.Component = seedMenu.Component;
            menu.Sort = seedMenu.Sort;
            menu.IsVisible = seedMenu.IsVisible;
            menu.IsEnabled = seedMenu.IsEnabled;
            menu.Description = seedMenu.Description;
            menu.IsDeleted = false;
            menu.DeletionTime = null;
            menu.DeleterId = null;
        }

        foreach (var seedMenu in PlatformAuthorizationSeedCatalog.Menus)
        {
            var menu = existingMenus[seedMenu.Code];
            menu.ParentId = seedMenu.ParentCode is null ? null : existingMenus[seedMenu.ParentCode].Id;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsurePermissionsAsync(CancellationToken cancellationToken)
    {
        var menuIds = await dbContext.Menus
            .IgnoreQueryFilters()
            .ToDictionaryAsync(menu => menu.Code, menu => menu.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingPermissions = await dbContext.Permissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(permission => permission.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var seedPermission in PlatformAuthorizationSeedCatalog.Permissions.OrderBy(permission => permission.Sort))
        {
            if (!menuIds.TryGetValue(seedPermission.MenuCode, out var menuId))
            {
                throw new InvalidOperationException($"Menu '{seedPermission.MenuCode}' was not found while seeding permissions.");
            }

            if (!existingPermissions.TryGetValue(seedPermission.Code, out var permission))
            {
                permission = new PlatformPermission();
                await dbContext.Permissions.AddAsync(permission, cancellationToken);
                existingPermissions[seedPermission.Code] = permission;
            }

            permission.MenuId = menuId;
            permission.Code = seedPermission.Code;
            permission.Name = seedPermission.Name;
            permission.Resource = seedPermission.Resource;
            permission.Action = seedPermission.Action;
            permission.PermissionType = seedPermission.PermissionType;
            permission.Sort = seedPermission.Sort;
            permission.IsSystem = seedPermission.IsSystem;
            permission.IsEnabled = seedPermission.IsEnabled;
            permission.Description = seedPermission.Description;
            permission.IsDeleted = false;
            permission.DeletionTime = null;
            permission.DeleterId = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var rolePermission in PlatformRolePermissionCatalog.RolePermissions)
        {
            var role = await roleManager.FindByNameAsync(rolePermission.Key);
            if (role is not null)
            {
                continue;
            }

            var createRoleResult = await roleManager.CreateAsync(new ApplicationRole(rolePermission.Key));
            if (!createRoleResult.Succeeded)
            {
                var errors = string.Join(", ", createRoleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to create role '{rolePermission.Key}': {errors}");
            }
        }
    }

    private async Task EnsureRolePermissionGrantsAsync(CancellationToken cancellationToken)
    {
        var roleIds = await roleManager.Roles
            .AsNoTracking()
            .Where(role => role.Name != null)
            .ToDictionaryAsync(role => role.Name!, role => role.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var permissionIds = await dbContext.Permissions
            .AsNoTracking()
            .ToDictionaryAsync(permission => permission.Code, permission => permission.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var rolePermissions in PlatformRolePermissionCatalog.RolePermissions)
        {
            if (!roleIds.TryGetValue(rolePermissions.Key, out var roleId))
            {
                continue;
            }

            var desiredPermissionIds = rolePermissions.Value
                .Where(permissionIds.ContainsKey)
                .Select(permissionCode => permissionIds[permissionCode])
                .ToHashSet();

            var existingGrants = await dbContext.RolePermissionGrants
                .Where(grant => grant.RoleId == roleId)
                .ToListAsync(cancellationToken);

            dbContext.RolePermissionGrants.RemoveRange(
                existingGrants.Where(grant => !desiredPermissionIds.Contains(grant.PermissionId)));

            var existingPermissionIds = existingGrants.Select(grant => grant.PermissionId).ToHashSet();
            foreach (var permissionId in desiredPermissionIds.Where(permissionId => !existingPermissionIds.Contains(permissionId)))
            {
                dbContext.RolePermissionGrants.Add(new PlatformRolePermissionGrant
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAdminUserAsync()
    {
        var admin = _seedOptions.Admin;
        var adminUser = await userManager.FindByNameAsync(admin.UserName)
            ?? await userManager.FindByEmailAsync(admin.Email);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = admin.UserName,
                Email = admin.Email,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(adminUser, admin.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join(", ", createUserResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to seed admin user: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, PlatformRoles.Admin))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, PlatformRoles.Admin);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addToRoleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to assign admin role: {errors}");
            }
        }
    }
}



