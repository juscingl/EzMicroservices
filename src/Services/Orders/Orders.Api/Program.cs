using BuildingBlocks.DependencyInjection;
using BuildingBlocks.Messaging.DependencyInjection;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Commands;
using Orders.Application.Dtos;
using Orders.Application.Services;
using Orders.Infrastructure.DependencyInjection;
using Orders.Infrastructure.EntityFrameworkCore.DbContexts;

const int DefaultSearchSize = 20;
const int MaxSearchSize = 100;

var builder = WebApplication.CreateBuilder(args);
// 加载 Nacos 配置并初始化统一可观测能力。
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("orders-api");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPlatformExceptionHandling();
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
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// 创建订单：校验输入后调用应用服务完成持久化。
app.MapPost("/orders", async (PlaceOrderRequest request, IOrderService orderService, CancellationToken cancellationToken) =>
{
    if (request.Lines.Count == 0 || request.Lines.Any(line => line.Quantity <= 0 || line.UnitPrice <= 0))
    {
        return Results.BadRequest("Order lines are required and each line must contain positive quantity and unitPrice.");
    }

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

// 订单搜索：统一控制查询条数上限，避免过大请求。
app.MapGet("/orders/search", async (
    string? keyword,
    Guid? customerId,
    int? size,
    IOrderService orderService,
    CancellationToken cancellationToken) =>
{
    var effectiveSize = size ?? DefaultSearchSize;
    if (effectiveSize <= 0 || effectiveSize > MaxSearchSize)
    {
        return Results.BadRequest($"size must be between 1 and {MaxSearchSize}.");
    }

    var results = await orderService.SearchAsync(keyword, customerId, effectiveSize, cancellationToken);
    return Results.Ok(results);
})
.RequireAuthorization(PlatformAuthorizationPolicies.OrdersRead);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

using (var scope = app.Services.CreateScope())
{
    // 启动时自动执行数据库迁移，确保表结构已就绪。
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();

internal sealed record PlaceOrderRequest(Guid CustomerId, List<OrderLineRequest> Lines);
internal sealed record OrderLineRequest(Guid ProductId, int Quantity, decimal UnitPrice);
