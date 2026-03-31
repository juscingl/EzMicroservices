using System.Text.Json;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("web-bff");

builder.Services.AddEndpointsApiExplorer();
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
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePlatformObservability();
app.UseAuthentication();
app.UseAuthorization();

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
    if (!orderResponse.IsSuccessStatusCode)
    {
        return Results.Problem(detail: "Order not found", statusCode: StatusCodes.Status404NotFound);
    }

    using var orderStream = await orderResponse.Content.ReadAsStreamAsync(ct);
    var order = await JsonSerializer.DeserializeAsync<JsonElement>(orderStream, cancellationToken: ct);

    var inventory = new List<object>();
    if (order.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
    {
        foreach (var item in items.EnumerateArray())
        {
            if (item.TryGetProperty("productId", out var sku) && sku.ValueKind == JsonValueKind.String && Guid.TryParse(sku.GetString(), out var skuId))
            {
                using var inventoryRequest = CreateAuthorizedRequest(HttpMethod.Get, $"/inventory/{skuId}", httpContext);
                var invResp = await inventoryClient.SendAsync(inventoryRequest, ct);
                if (invResp.IsSuccessStatusCode)
                {
                    var invJson = await invResp.Content.ReadAsStringAsync(ct);
                    inventory.Add(JsonSerializer.Deserialize<JsonElement>(invJson));
                }
            }
        }
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
