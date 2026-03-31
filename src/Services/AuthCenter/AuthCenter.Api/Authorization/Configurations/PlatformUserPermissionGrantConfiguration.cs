using AuthCenter.Api.Authorization.Entities;
using AuthCenter.Api.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Api.Authorization.Configurations;

public sealed class PlatformUserPermissionGrantConfiguration : IEntityTypeConfiguration<PlatformUserPermissionGrant>
{
    public void Configure(EntityTypeBuilder<PlatformUserPermissionGrant> builder)
    {
        builder.ToTable("auth_user_permission_grants");
        builder.HasKey(grant => new { grant.UserId, grant.PermissionId });
        builder.HasQueryFilter(grant => !grant.Permission.IsDeleted);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(grant => grant.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(grant => grant.Permission)
            .WithMany(permission => permission.UserGrants)
            .HasForeignKey(grant => grant.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
