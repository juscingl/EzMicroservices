namespace AuthCenter.Api.Endpoints;

/// <summary>
/// AuthCenter 端点映射扩展。
/// </summary>
public static class AuthCenterEndpointRouteBuilderExtensions
{
    /// <summary>
    /// 统一映射认证与权限管理端点。
    /// </summary>
    public static IEndpointRouteBuilder MapAuthCenterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapConnectEndpoints();
        endpoints.MapIdentityManagementEndpoints();
        return endpoints;
    }
}
