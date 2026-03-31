using AuthCenter.Api.DependencyInjection;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("auth-center");

builder.Services.AddPlatformNacos(builder.Configuration, "auth-center");
builder.Services.AddAuthCenter(builder.Configuration);

var app = builder.Build();
app.UsePlatformObservability();
app.UseAuthCenter();
app.Run();
