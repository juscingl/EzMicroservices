namespace AuthCenter.Api.Services;

[Obsolete("Use IPermissionGrantResolver instead.")]
/// <summary>
/// 旧版角色权限解析实现（已废弃），内部转调新解析器。
/// </summary>
public sealed class RolePermissionResolver(IPermissionGrantResolver permissionGrantResolver) : IRolePermissionResolver
{
    /// <summary>
    /// 根据角色名集合解析权限编码。
    /// </summary>
    public Task<IReadOnlyCollection<string>> ResolveAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        return permissionGrantResolver.GetRolePermissionCodesAsync(roleNames, cancellationToken);
    }
}
