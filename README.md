# EzMicroservices

一个基于 **.NET 8** 的微服务示例工程，覆盖了从认证、网关、BFF 到业务服务（订单/库存/支付）的完整后端链路。

## 架构概览

- **AuthCenter**：统一认证授权中心（OIDC/OAuth2）。
- **ApiGateway (YARP)**：统一入口，按路由转发到各下游服务。
- **WebBff**：面向前端的聚合接口，减少前端多次调用。
- **Orders / Inventory / Payments**：核心业务微服务。
- **BuildingBlocks**：公共基础能力（安全、消息、EF Core、可观测、配置中心、搜索等）。

## 技术栈

- .NET 8 / ASP.NET Core Minimal API
- Entity Framework Core + PostgreSQL
- RabbitMQ（集成事件）
- Nacos（配置与服务注册）
- Elasticsearch + Logstash + Kibana + Filebeat（日志与观测）
- YARP（反向代理）
- Docker Compose（本地联调）

## 快速启动

### 1) 环境准备

- Docker / Docker Compose
- .NET SDK 8

### 2) 配置环境变量（可选）

可通过 `.env` 传入，例如：

```env
POSTGRES_HOST=postgres
POSTGRES_PORT=5432
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
SEED_ADMIN_PASSWORD=Admin123!
```

### 3) 启动依赖和服务

```bash
docker compose -f docker-compose.db.yml up -d
docker compose up -d --build
```

### 4) 访问入口

- API Gateway: `http://localhost:5080`
- AuthCenter: `http://localhost:5085`
- Orders API: `http://localhost:5239`
- Inventory API: `http://localhost:5270`
- Payments API: `http://localhost:5066`
- Web BFF: `http://localhost:5097`
- Kibana: `http://localhost:5601`
- Nacos: `http://localhost:8848`
- RabbitMQ 管理台: `http://localhost:15672`

## 典型调用路径

1. 客户端经 `ApiGateway` 访问业务接口。
2. `ApiGateway` 完成认证授权后转发。
3. 业务服务按领域拆分处理请求。
4. `Payments` 通过 RabbitMQ 订阅订单事件实现异步处理。
5. `WebBff` 聚合订单与库存数据供前端一次读取。

## 开发与测试

```bash
dotnet restore
dotnet build
dotnet test
```

## 工程约定

- 服务启动时自动执行数据库迁移。
- 统一启用 ProblemDetails 和异常处理管道。
- 统一暴露健康检查端点：`/health`、`/health/ready`。

## 后续优化建议

- 增加契约测试（consumer-driven contracts）
- 为 BFF 增加缓存和熔断策略
- 细化服务级别 SLO 与告警阈值
