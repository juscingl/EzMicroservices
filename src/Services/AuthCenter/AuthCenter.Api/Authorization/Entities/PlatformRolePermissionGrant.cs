using BuildingBlocks.Auditing;

namespace AuthCenter.Api.Authorization.Entities;

public sealed class PlatformRolePermissionGrant : ICreationAuditedObject
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public PlatformPermission Permission { get; set; } = null!;
}
