using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Api.Identity;

/// <summary>
/// 认证中心角色实体，扩展 IdentityRole 并补充审计字段。
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>, IFullAuditedObject
{
    /// <summary>
    /// 无参构造，供 EF Core 使用。
    /// </summary>
    public ApplicationRole()
    {
    }

    /// <summary>
    /// 按角色名创建角色。
    /// </summary>
    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    /// <summary>
    /// 角色描述。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 角色编码（用于系统内稳定引用）。
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 显示排序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否启用角色。
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
