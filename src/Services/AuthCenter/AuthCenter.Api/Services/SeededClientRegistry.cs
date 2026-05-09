using AuthCenter.Api.Options;
using Microsoft.Extensions.Options;

namespace AuthCenter.Api.Services;

/// <summary>
/// 预置客户端注册表实现，基于配置提供客户端定义查询能力。
/// </summary>
public sealed class SeededClientRegistry(IOptions<AuthCenterSeedOptions> seedOptions) : ISeededClientRegistry
{
    private readonly AuthCenterSeedOptions _seedOptions = seedOptions.Value;

    /// <summary>
    /// 返回全部预置客户端。
    /// </summary>
    public IReadOnlyCollection<AuthCenterClientSeedOptions> GetAll()
    {
        return _seedOptions.Clients;
    }

    /// <summary>
    /// 按客户端标识查找预置客户端定义。
    /// </summary>
    public AuthCenterClientSeedOptions? Find(string clientId)
    {
        return _seedOptions.Clients.FirstOrDefault(client =>
            string.Equals(client.ClientId, clientId, StringComparison.OrdinalIgnoreCase));
    }
}
