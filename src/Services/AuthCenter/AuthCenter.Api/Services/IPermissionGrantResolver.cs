using AuthCenter.Api.Models.Authorization;

namespace AuthCenter.Api.Services;

/// <summary>
/// 权限授予解析器，负责从角色/用户关系中汇总有效权限与菜单。
/// </summary>
public interface IPermissionGrantResolver
{
    /// <summary>
    /// 根据角色集合解析角色权限编码。
    /// </summary>
    Task<IReadOnlyCollection<string>> GetRolePermissionCodesAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户直接授权的权限编码。
    /// </summary>
    Task<IReadOnlyCollection<string>> GetUserDirectPermissionCodesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 汇总用户最终权限（角色权限 + 用户直授权限）。
    /// </summary>
    Task<IReadOnlyCollection<string>> GetUserPermissionCodesAsync(
        Guid userId,
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据权限编码构建可见菜单树。
    /// </summary>
    Task<IReadOnlyCollection<MenuNodeResponse>> GetMenusAsync(
        IEnumerable<string> permissionCodes,
        CancellationToken cancellationToken = default);
}
