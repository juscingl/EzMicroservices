using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Api.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>, IFullAuditedObject
{
    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTimeOffset? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletionTime { get; set; }

    public Guid? DeleterId { get; set; }
}
