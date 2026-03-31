namespace AuthCenter.Api.Endpoints;

public static class AuthCenterEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapAuthCenterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapConnectEndpoints();
        endpoints.MapIdentityManagementEndpoints();
        return endpoints;
    }
}
