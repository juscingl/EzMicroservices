using BuildingBlocks.Contracts.IntegrationEvents;
using BuildingBlocks.Contracts.Messaging;
using BuildingBlocks.Messaging.DependencyInjection;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Payments.Application.IntegrationHandlers;
using Payments.Application.Services;
using Payments.Infrastructure.DependencyInjection;
using Payments.Infrastructure.EntityFrameworkCore.DbContexts;

var builder = WebApplication.CreateBuilder(args);
// 加载 Nacos 配置并初始化统一可观测能力。
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("payments-api");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen();
builder.Services.AddPaymentsInfrastructure(builder.Configuration);
builder.Services.AddPlatformMessaging(builder.Configuration, configure =>
    configure.AddConsumer<OrderCreatedIntegrationEvent, OrderCreatedPaymentHandler>(
        queueName: "payments.order-created",
        routingKey: IntegrationRoutingKeys.OrdersCreated));
builder.Services.AddPlatformNacos(builder.Configuration, "payments-api");
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformAuthorization();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHealthChecks().AddDbContextCheck<PaymentsDbContext>();

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

// 手工支付接口：用于主动发起支付扣款。
app.MapPost("/payments", async (CreatePaymentRequest request, IPaymentService paymentService, CancellationToken cancellationToken) =>
{
    var payment = await paymentService.CaptureAsync(request.OrderId, request.Amount, request.Currency ?? "CNY", cancellationToken);
    return Results.Ok(new { payment.OrderId, payment.Amount, payment.Currency, payment.Status });
})
.RequireAuthorization(PlatformAuthorizationPolicies.PaymentsWrite);

app.MapGet("/payments/{orderId:guid}", async (Guid orderId, IPaymentService paymentService, CancellationToken cancellationToken) =>
{
    var payment = await paymentService.GetAsync(orderId, cancellationToken);
    return payment is null ? Results.NotFound() : Results.Ok(payment);
})
.RequireAuthorization(PlatformAuthorizationPolicies.PaymentsRead);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

using (var scope = app.Services.CreateScope())
{
    // 启动时自动执行数据库迁移，确保表结构已就绪。
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();

internal sealed record CreatePaymentRequest(Guid OrderId, decimal Amount, string? Currency);
