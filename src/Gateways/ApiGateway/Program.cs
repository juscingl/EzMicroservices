using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("api-gateway");

builder.Services.AddPlatformNacos(builder.Configuration, "api-gateway");
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformAuthorization();
builder.Services.AddReverseProxy().LoadFromMemory(
    routes:
    [
        new RouteConfig
        {
            RouteId = "oidc-discovery",
            ClusterId = "auth-center",
            AuthorizationPolicy = "anonymous",
            Match = new RouteMatch { Path = "/.well-known/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "oidc-connect",
            ClusterId = "auth-center",
            AuthorizationPolicy = "anonymous",
            Match = new RouteMatch { Path = "/connect/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "auth-center",
            ClusterId = "auth-center",
            AuthorizationPolicy = PlatformAuthorizationPolicies.AuthenticatedUser,
            Match = new RouteMatch { Path = "/auth/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "orders",
            ClusterId = "orders",
            AuthorizationPolicy = PlatformAuthorizationPolicies.AuthenticatedUser,
            Match = new RouteMatch { Path = "/orders/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "inventory",
            ClusterId = "inventory",
            AuthorizationPolicy = PlatformAuthorizationPolicies.AuthenticatedUser,
            Match = new RouteMatch { Path = "/inventory/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "payments",
            ClusterId = "payments",
            AuthorizationPolicy = PlatformAuthorizationPolicies.AuthenticatedUser,
            Match = new RouteMatch { Path = "/payments/{**catchAll}" }
        },
        new RouteConfig
        {
            RouteId = "bff",
            ClusterId = "bff",
            AuthorizationPolicy = PlatformAuthorizationPolicies.AuthenticatedUser,
            Match = new RouteMatch { Path = "/bff/{**catchAll}" }
        }
    ],
    clusters:
    [
        new ClusterConfig
        {
            ClusterId = "auth-center",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["d1"] = new() { Address = builder.Configuration.GetValue<string>("Services:AuthCenter") ?? "http://authcenter-api" }
            }
        },
        new ClusterConfig
        {
            ClusterId = "orders",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["d1"] = new() { Address = builder.Configuration.GetValue<string>("Services:Orders") ?? "http://orders-api" }
            }
        },
        new ClusterConfig
        {
            ClusterId = "inventory",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["d1"] = new() { Address = builder.Configuration.GetValue<string>("Services:Inventory") ?? "http://inventory-api" }
            }
        },
        new ClusterConfig
        {
            ClusterId = "payments",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["d1"] = new() { Address = builder.Configuration.GetValue<string>("Services:Payments") ?? "http://payments-api" }
            }
        },
        new ClusterConfig
        {
            ClusterId = "bff",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                ["d1"] = new() { Address = builder.Configuration.GetValue<string>("Services:Bff") ?? "http://webbff" }
            }
        }
    ]);

var app = builder.Build();

app.UsePlatformObservability();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
