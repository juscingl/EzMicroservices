using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Api.Identity;

/// <summary>
/// 认证中心用户实体，扩展 IdentityUser 并补充审计字段。
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>, IFullAuditedObject
{
    /// <summary>
    /// 显示名称（用于后台列表展示）。
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用账号。
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间（UTC）。
    /// </summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人标识。
    /// </summary>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 最后修改时间（UTC）。
    /// </summary>
    public DateTimeOffset? LastModificationTime { get; set; }

    /// <summary>
    /// 最后修改人标识。
    /// </summary>
    public Guid? LastModifierId { get; set; }

    /// <summary>
    /// 软删除标记。
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间（UTC）。
    /// </summary>
    public DateTimeOffset? DeletionTime { get; set; }

    /// <summary>
    /// 删除人标识。
    /// </summary>
    public Guid? DeleterId { get; set; }
}
