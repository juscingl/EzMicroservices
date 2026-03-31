using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Api.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>, IFullAuditedObject
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTimeOffset? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletionTime { get; set; }

    public Guid? DeleterId { get; set; }
}
