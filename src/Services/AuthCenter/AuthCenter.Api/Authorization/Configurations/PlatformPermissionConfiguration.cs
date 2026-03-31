using AuthCenter.Api.Authorization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Api.Authorization.Configurations;

public sealed class PlatformPermissionConfiguration : IEntityTypeConfiguration<PlatformPermission>
{
    public void Configure(EntityTypeBuilder<PlatformPermission> builder)
    {
        builder.ToTable("auth_platform_permissions");
        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Code)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(permission => permission.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(permission => permission.Resource)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(permission => permission.Action)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(permission => permission.PermissionType)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(permission => permission.Description).HasMaxLength(512);

        builder.HasIndex(permission => permission.Code).IsUnique();
        builder.HasIndex(permission => new { permission.MenuId, permission.Sort });

        builder.HasOne(permission => permission.Menu)
            .WithMany(menu => menu.Permissions)
            .HasForeignKey(permission => permission.MenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
