using BuildingBlocks.Domain;

namespace AuthCenter.Api.Authorization.Entities;

public sealed class PlatformPermission : FullAuditedEntity
{
    public Guid? MenuId { get; set; }

    public PlatformMenu? Menu { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Resource { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string PermissionType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Sort { get; set; }

    public bool IsSystem { get; set; } = true;

    public bool IsEnabled { get; set; } = true;

    public ICollection<PlatformRolePermissionGrant> RoleGrants { get; set; } = [];

    public ICollection<PlatformUserPermissionGrant> UserGrants { get; set; } = [];
}
