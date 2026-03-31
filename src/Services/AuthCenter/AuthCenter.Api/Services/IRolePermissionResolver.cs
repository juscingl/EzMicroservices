namespace AuthCenter.Api.Services;

[Obsolete("Use IPermissionGrantResolver instead.")]
public interface IRolePermissionResolver
{
    Task<IReadOnlyCollection<string>> ResolveAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken = default);
}
