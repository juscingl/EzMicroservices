using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Api.Authorization.Configurations;

public sealed class PlatformRolePermissionGrantConfiguration : IEntityTypeConfiguration<PlatformRolePermissionGrant>
{
    public void Configure(EntityTypeBuilder<PlatformRolePermissionGrant> builder)
    {
        builder.ToTable("auth_role_permission_grants");
        builder.HasKey(grant => new { grant.RoleId, grant.PermissionId });
        builder.HasQueryFilter(grant => !grant.Permission.IsDeleted);

        builder.HasOne<ApplicationRole>()
            .WithMany()
            .HasForeignKey(grant => grant.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(grant => grant.Permission)
            .WithMany(permission => permission.RoleGrants)
            .HasForeignKey(grant => grant.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
