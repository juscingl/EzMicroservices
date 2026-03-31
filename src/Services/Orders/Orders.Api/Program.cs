using BuildingBlocks.Messaging.DependencyInjection;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Orders.Application.Commands;
using Orders.Application.Dtos;
using Orders.Application.Services;
using Orders.Infrastructure.DependencyInjection;
using Orders.Infrastructure.EntityFrameworkCore.DbContexts;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("orders-api");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOrdersInfrastructure(builder.Configuration);
builder.Services.AddPlatformMessaging(builder.Configuration);
builder.Services.AddPlatformNacos(builder.Configuration, "orders-api");
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformAuthorization();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHealthChecks().AddDbContextCheck<OrdersDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePlatformObservability();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/orders", async (PlaceOrderRequest request, IOrderService orderService, CancellationToken cancellationToken) =>
{
    var command = new PlaceOrderCommand(
        request.CustomerId,
        request.Lines.Select(line => new OrderLineDto(line.ProductId, line.Quantity, line.UnitPrice)).ToArray());

    var id = await orderService.PlaceAsync(command, cancellationToken);
    return Results.Created($"/orders/{id}", new { id });
})
.RequireAuthorization(PlatformAuthorizationPolicies.OrdersWrite);

app.MapGet("/orders/{id:guid}", async (Guid id, IOrderService orderService, CancellationToken cancellationToken) =>
{
    var order = await orderService.GetAsync(id, cancellationToken);
    return order is null ? Results.NotFound() : Results.Ok(order);
})
.RequireAuthorization(PlatformAuthorizationPolicies.OrdersRead);

app.MapGet("/orders/search", async (
    string? keyword,
    Guid? customerId,
    int? size,
    IOrderService orderService,
    CancellationToken cancellationToken) =>
{
    var results = await orderService.SearchAsync(keyword, customerId, size ?? 20, cancellationToken);
    return Results.Ok(results);
})
.RequireAuthorization(PlatformAuthorizationPolicies.OrdersRead);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();

internal sealed record PlaceOrderRequest(Guid CustomerId, List<OrderLineRequest> Lines);
internal sealed record OrderLineRequest(Guid ProductId, int Quantity, decimal UnitPrice);
