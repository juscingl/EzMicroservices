using BuildingBlocks.DependencyInjection;
using AuthCenter.Api.DependencyInjection;
using BuildingBlocks.Nacos.Configuration;
using BuildingBlocks.Nacos.DependencyInjection;
using BuildingBlocks.Observability.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// 加载 Nacos 配置并初始化统一可观测能力。
builder.Configuration.AddNacosJsonConfiguration(builder.Configuration);
builder.AddPlatformObservability("auth-center");

builder.Services.AddPlatformNacos(builder.Configuration, "auth-center");
builder.Services.AddPlatformExceptionHandling();
builder.Services.AddAuthCenter(builder.Configuration);

var app = builder.Build();
// 启用日志、认证授权及认证中心端点。
app.UsePlatformObservability();
app.UseExceptionHandler();
app.UseAuthCenter();
app.Run();
