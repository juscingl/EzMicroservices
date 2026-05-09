namespace AuthCenter.Api.Services;

/// <summary>
/// 认证中心数据播种接口。
/// </summary>
public interface IAuthCenterDataSeeder
{
    /// <summary>
    /// 初始化默认用户、角色、权限及客户端数据。
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
