using BuildingBlocks.Auditing;

namespace AuthCenter.Api.Authorization.Entities;

public sealed class PlatformUserPermissionGrant : ICreationAuditedObject
{
    public Guid UserId { get; set; }

    public Guid PermissionId { get; set; }

    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public PlatformPermission Permission { get; set; } = null!;
}
