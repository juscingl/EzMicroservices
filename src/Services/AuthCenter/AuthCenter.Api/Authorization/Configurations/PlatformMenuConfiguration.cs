using AuthCenter.Api.Authorization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthCenter.Api.Authorization.Configurations;

public sealed class PlatformMenuConfiguration : IEntityTypeConfiguration<PlatformMenu>
{
    public void Configure(EntityTypeBuilder<PlatformMenu> builder)
    {
        builder.ToTable("auth_platform_menus");
        builder.HasKey(menu => menu.Id);

        builder.Property(menu => menu.Code)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(menu => menu.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(menu => menu.Route)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(menu => menu.Icon).HasMaxLength(128);
        builder.Property(menu => menu.Component).HasMaxLength(256);
        builder.Property(menu => menu.Description).HasMaxLength(512);

        builder.HasIndex(menu => menu.Code).IsUnique();
        builder.HasIndex(menu => new { menu.ParentId, menu.Sort });

        builder.HasOne(menu => menu.Parent)
            .WithMany(menu => menu.Children)
            .HasForeignKey(menu => menu.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
