using BuildingBlocks.DependencyInjection;
using System.Text.Json;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Microsoft.Net.Http.Headers;

const int InventoryHttpTimeoutSeconds = 5;

var builder = WebApplication.CreateBuilder(args);
// 加载 Nacos 配置并初始化统一可观测能力。
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("web-bff");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen();
builder.Services.AddPlatformNacos(builder.Configuration, "web-bff");
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformAuthorization();

builder.Services.AddHttpClient("orders", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Services:Orders") ?? "http://orders-api");
});

builder.Services.AddHttpClient("inventory", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Services:Inventory") ?? "http://inventory-api");
    client.Timeout = TimeSpan.FromSeconds(InventoryHttpTimeoutSeconds);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePlatformObservability();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// 聚合订单与库存信息，向前端提供单次读取接口。
app.MapGet("/bff/orders/{id:guid}", async (
    Guid id,
    IHttpClientFactory factory,
    HttpContext httpContext,
    CancellationToken ct) =>
{
    var ordersClient = factory.CreateClient("orders");
    var inventoryClient = factory.CreateClient("inventory");

    using var orderRequest = CreateAuthorizedRequest(HttpMethod.Get, $"/orders/{id}", httpContext);
    var orderResponse = await ordersClient.SendAsync(orderRequest, ct);
    if (orderResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return Results.Problem(detail: "Order not found", statusCode: StatusCodes.Status404NotFound);
    }

    if (!orderResponse.IsSuccessStatusCode)
    {
        return Results.Problem(detail: "Orders service unavailable", statusCode: StatusCodes.Status502BadGateway);
    }

    using var orderStream = await orderResponse.Content.ReadAsStreamAsync(ct);
    var order = await JsonSerializer.DeserializeAsync<JsonElement>(orderStream, cancellationToken: ct);

    var inventory = new List<JsonElement>();
    if (order.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
    {
        var productIds = items.EnumerateArray()
            .Select(item => item.TryGetProperty("productId", out var productId)
                && productId.ValueKind == JsonValueKind.String
                && Guid.TryParse(productId.GetString(), out var parsedProductId)
                    ? parsedProductId
                    : (Guid?)null)
            .Where(productId => productId.HasValue)
            .Select(productId => productId!.Value)
            .Distinct()
            .ToArray();

        var inventoryTasks = productIds
            .Select(productId => FetchInventoryItemAsync(inventoryClient, productId, httpContext, ct))
            .ToArray();
        var inventoryResults = await Task.WhenAll(inventoryTasks);
        inventory.AddRange(inventoryResults.Where(result => result.HasValue).Select(result => result!.Value));
    }

    return Results.Ok(new { order, inventory });
})
.RequireAuthorization(PlatformAuthorizationPolicies.OrdersRead);

app.Run();

static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string uri, HttpContext httpContext)
{
    var request = new HttpRequestMessage(method, uri);
    if (httpContext.Request.Headers.Authorization is { Count: > 0 } authHeader)
    {
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, authHeader.ToString());
    }

    return request;
}

static async Task<JsonElement?> FetchInventoryItemAsync(
    HttpClient inventoryClient,
    Guid productId,
    HttpContext httpContext,
    CancellationToken cancellationToken)
{
    using var inventoryRequest = CreateAuthorizedRequest(HttpMethod.Get, $"/inventory/{productId}", httpContext);
    using var inventoryResponse = await inventoryClient.SendAsync(inventoryRequest, cancellationToken);
    if (!inventoryResponse.IsSuccessStatusCode)
    {
        return null;
    }

    using var inventoryStream = await inventoryResponse.Content.ReadAsStreamAsync(cancellationToken);
    var inventoryItem = await JsonSerializer.DeserializeAsync<JsonElement>(inventoryStream, cancellationToken: cancellationToken);
    return inventoryItem;
}
