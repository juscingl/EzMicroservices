using AuthCenter.Api.Authorization.Configurations;
using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.Identity;
using BuildingBlocks.Auditing;
using BuildingBlocks.EntityFrameworkCore.Auditing;
using BuildingBlocks.Uow;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace AuthCenter.Api.EntityFrameworkCore;

public sealed class AuthCenterDbContext(
    DbContextOptions<AuthCenterDbContext> options,
    ICurrentUserAccessor currentUserAccessor)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IUnitOfWork
{
    private ICurrentUserAccessor CurrentUserAccessor { get; } = currentUserAccessor;

    public DbSet<PlatformMenu> Menus => Set<PlatformMenu>();

    public DbSet<PlatformPermission> Permissions => Set<PlatformPermission>();

    public DbSet<PlatformRolePermissionGrant> RolePermissionGrants => Set<PlatformRolePermissionGrant>();

    public DbSet<PlatformUserPermissionGrant> UserPermissionGrants => Set<PlatformUserPermissionGrant>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("auth_users");
        builder.Entity<ApplicationRole>().ToTable("auth_roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("auth_user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("auth_user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("auth_user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("auth_user_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("auth_role_claims");

        builder.Entity<OpenIddictEntityFrameworkCoreApplication<Guid>>().ToTable("oidc_applications");
        builder.Entity<OpenIddictEntityFrameworkCoreAuthorization<Guid>>().ToTable("oidc_authorizations");
        builder.Entity<OpenIddictEntityFrameworkCoreScope<Guid>>().ToTable("oidc_scopes");
        builder.Entity<OpenIddictEntityFrameworkCoreToken<Guid>>().ToTable("oidc_tokens");

        builder.ApplyConfiguration(new PlatformMenuConfiguration());
        builder.ApplyConfiguration(new PlatformPermissionConfiguration());
        builder.ApplyConfiguration(new PlatformRolePermissionGrantConfiguration());
        builder.ApplyConfiguration(new PlatformUserPermissionGrantConfiguration());
        builder.ApplyPlatformConventions();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.ApplyPlatformAuditing(CurrentUserAccessor);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
