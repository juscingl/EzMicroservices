using AuthCenter.Api.Identity;

namespace AuthCenter.Api.Services;

/// <summary>
/// OpenIddict Principal 构建工厂接口。
/// </summary>
public interface IOpenIddictPrincipalFactory
{
    /// <summary>
    /// 为用户登录场景创建 Principal。
    /// </summary>
    OpenIddictPrincipalFactoryResult CreateForUser(
        ApplicationUser user,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        IEnumerable<string> requestedScopes);

    /// <summary>
    /// 为客户端凭据场景创建 Principal。
    /// </summary>
    OpenIddictPrincipalFactoryResult CreateForClient(
        string clientId,
        IEnumerable<string> requestedScopes);
}
