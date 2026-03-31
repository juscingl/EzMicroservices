using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;
using BuildingBlocks.Security.Authorization;
using BuildingBlocks.Security.DependencyInjection;
using Inventory.Application.Services;
using Inventory.Domain.Repositories;
using Inventory.Infrastructure.DependencyInjection;
using Inventory.Infrastructure.EntityFrameworkCore.DbContexts;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("inventory-api");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddPlatformNacos(builder.Configuration, "inventory-api");
builder.Services.AddPlatformAuthentication(builder.Configuration);
builder.Services.AddPlatformAuthorization();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddHealthChecks().AddDbContextCheck<InventoryDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePlatformObservability();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/inventory/{skuId:guid}/adjust", async (
    Guid skuId,
    AdjustmentRequest request,
    IInventoryService inventoryService,
    CancellationToken cancellationToken) =>
{
    var quantity = await inventoryService.AdjustAsync(skuId, request.Delta, cancellationToken);
    return Results.Ok(new { skuId, quantity });
})
.RequireAuthorization(PlatformAuthorizationPolicies.InventoryWrite);

app.MapGet("/inventory/{skuId:guid}", async (
    Guid skuId,
    IInventoryRepository inventoryRepository,
    CancellationToken cancellationToken) =>
{
    var stockItem = await inventoryRepository.FindBySkuIdAsync(skuId, cancellationToken);
    return stockItem is null ? Results.NotFound() : Results.Ok(stockItem);
})
.RequireAuthorization(PlatformAuthorizationPolicies.InventoryRead);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();

internal sealed record AdjustmentRequest(int Delta);
