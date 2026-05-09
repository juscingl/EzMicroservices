namespace AuthCenter.Api.Services;

[Obsolete("Use IPermissionGrantResolver instead.")]
/// <summary>
/// 旧版角色权限解析接口（已废弃），仅保留兼容用途。
/// </summary>
public interface IRolePermissionResolver
{
    /// <summary>
    /// 根据角色名集合解析权限编码。
    /// </summary>
    Task<IReadOnlyCollection<string>> ResolveAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);
}
