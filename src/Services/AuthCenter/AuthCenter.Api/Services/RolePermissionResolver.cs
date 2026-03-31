namespace AuthCenter.Api.Services;

[Obsolete("Use IPermissionGrantResolver instead.")]
public sealed class RolePermissionResolver(IPermissionGrantResolver permissionGrantResolver) : IRolePermissionResolver
{
    public Task<IReadOnlyCollection<string>> ResolveAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        return permissionGrantResolver.GetRolePermissionCodesAsync(roleNames, cancellationToken);
    }
}
