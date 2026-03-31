using AuthCenter.Api.Identity;

namespace AuthCenter.Api.Services;

public interface IOpenIddictPrincipalFactory
{
    OpenIddictPrincipalFactoryResult CreateForUser(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        IEnumerable<string> requestedScopes);

    OpenIddictPrincipalFactoryResult CreateForClient(
        string clientId,
        IEnumerable<string> requestedScopes);
}
