using System.Security.Claims;
using AuthCenter.Api.Models;
using AuthCenter.Api.Models.Authorization;
using AuthCenter.Api.Services;
using BuildingBlocks.Security.Authorization;

namespace AuthCenter.Api.Endpoints;

public static class IdentityManagementEndpoints
{
    public static IEndpointRouteBuilder MapIdentityManagementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth")
            .WithTags("identity-management");

        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.AuthenticatedUser);

        group.MapGet("/me/permissions", GetCurrentUserPermissionsAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.AuthenticatedUser);

        group.MapGet("/me/menus", GetCurrentUserMenusAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.AuthenticatedUser);

        group.MapGet("/users", GetUsersAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.UsersRead);

        group.MapGet("/users/{id:guid}", GetUserAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.UsersRead);

        group.MapPost("/users", CreateUserAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.UsersWrite);

        group.MapPut("/users/{id:guid}/roles", UpdateUserRolesAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.UsersWrite);

        group.MapPut("/users/{id:guid}/permissions", UpdateUserPermissionsAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.UsersWrite);

        group.MapGet("/roles", GetRolesAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.RolesRead);

        group.MapPost("/roles", CreateRoleAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.RolesWrite);

        group.MapPut("/roles/{id:guid}/permissions", UpdateRolePermissionsAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.RolesWrite);

        group.MapGet("/menus", GetMenusAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.MenusRead);

        group.MapPost("/menus", CreateMenuAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.MenusWrite);

        group.MapPut("/menus/{id:guid}", UpdateMenuAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.MenusWrite);

        group.MapGet("/permissions", GetPermissionsAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.PermissionsRead);

        group.MapPost("/permissions", CreatePermissionAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.PermissionsWrite);

        group.MapPut("/permissions/{id:guid}", UpdatePermissionAsync)
            .RequireAuthorization(PlatformAuthorizationPolicies.PermissionsWrite);

        return endpoints;
    }

    private static async Task<IResult> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(principal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var profile = await authorizationManagementService.GetCurrentUserAsync(userId.Value, cancellationToken);
        return profile is null ? Results.Unauthorized() : Results.Ok(profile);
    }

    private static async Task<IResult> GetCurrentUserPermissionsAsync(
        ClaimsPrincipal principal,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(principal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var permissions = await authorizationManagementService.GetCurrentUserPermissionsAsync(userId.Value, cancellationToken);
        return Results.Ok(new { permissions });
    }

    private static async Task<IResult> GetCurrentUserMenusAsync(
        ClaimsPrincipal principal,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(principal);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var menus = await authorizationManagementService.GetCurrentUserMenusAsync(userId.Value, cancellationToken);
        return Results.Ok(menus);
    }

    private static async Task<IResult> GetUsersAsync(
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await authorizationManagementService.GetUsersAsync(cancellationToken));
    }

    private static async Task<IResult> GetUserAsync(
        Guid id,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        var user = await authorizationManagementService.GetUserAsync(id, cancellationToken);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authorizationManagementService.CreateUserAsync(request, cancellationToken);
            return Results.Created($"/auth/users/{user.Id}", user);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> UpdateUserRolesAsync(
        Guid id,
        UpdateUserRolesRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authorizationManagementService.UpdateUserRolesAsync(id, request, cancellationToken);
            return user is null ? Results.NotFound() : Results.Ok(user);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> UpdateUserPermissionsAsync(
        Guid id,
        UpdateUserPermissionsRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authorizationManagementService.UpdateUserPermissionsAsync(id, request, cancellationToken);
            return user is null ? Results.NotFound() : Results.Ok(user);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> GetRolesAsync(
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await authorizationManagementService.GetRolesAsync(cancellationToken));
    }

    private static async Task<IResult> CreateRoleAsync(
        CreateRoleRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await authorizationManagementService.CreateRoleAsync(request, cancellationToken);
            return Results.Created($"/auth/roles/{role.Id}", role);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> UpdateRolePermissionsAsync(
        Guid id,
        UpdateRolePermissionsRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var role = await authorizationManagementService.UpdateRolePermissionsAsync(id, request, cancellationToken);
            return role is null ? Results.NotFound() : Results.Ok(role);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> GetMenusAsync(
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await authorizationManagementService.GetMenusAsync(cancellationToken));
    }

    private static async Task<IResult> CreateMenuAsync(
        SaveMenuRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var menu = await authorizationManagementService.SaveMenuAsync(menuId: null, request, cancellationToken);
            return Results.Created($"/auth/menus/{menu.Id}", menu);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> UpdateMenuAsync(
        Guid id,
        SaveMenuRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var menu = await authorizationManagementService.SaveMenuAsync(id, request, cancellationToken);
            return Results.Ok(menu);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> GetPermissionsAsync(
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        return Results.Ok(await authorizationManagementService.GetPermissionsAsync(cancellationToken));
    }

    private static async Task<IResult> CreatePermissionAsync(
        SavePermissionRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var permission = await authorizationManagementService.SavePermissionAsync(permissionId: null, request, cancellationToken);
            return Results.Created($"/auth/permissions/{permission.Id}", permission);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static async Task<IResult> UpdatePermissionAsync(
        Guid id,
        SavePermissionRequest request,
        IAuthorizationManagementService authorizationManagementService,
        CancellationToken cancellationToken)
    {
        try
        {
            var permission = await authorizationManagementService.SavePermissionAsync(id, request, cancellationToken);
            return Results.Ok(permission);
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject);
        return Guid.TryParse(subject, out var userId) ? userId : null;
    }
}
