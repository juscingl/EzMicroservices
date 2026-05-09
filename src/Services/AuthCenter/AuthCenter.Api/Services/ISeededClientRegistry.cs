using AuthCenter.Api.Options;

namespace AuthCenter.Api.Services;

/// <summary>
/// 默认客户端注册表接口。
/// </summary>
public interface ISeededClientRegistry
{
    /// <summary>
    /// 获取全部预置客户端定义。
    /// </summary>
    IReadOnlyCollection<AuthCenterClientSeedOptions> GetAll();

    /// <summary>
    /// 按客户端标识查找预置客户端定义。
    /// </summary>
    AuthCenterClientSeedOptions? Find(string clientId);
}
