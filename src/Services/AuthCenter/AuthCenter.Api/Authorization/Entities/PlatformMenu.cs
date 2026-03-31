using BuildingBlocks.Domain;

namespace AuthCenter.Api.Authorization.Entities;

public sealed class PlatformMenu : FullAuditedEntity
{
    public Guid? ParentId { get; set; }

    public PlatformMenu? Parent { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string? Component { get; set; }

    public string? Description { get; set; }

    public int Sort { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool IsEnabled { get; set; } = true;

    public ICollection<PlatformMenu> Children { get; set; } = [];

    public ICollection<PlatformPermission> Permissions { get; set; } = [];
}
