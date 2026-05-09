namespace AuthCenter.Api.Options;

/// <summary>
/// 认证中心默认数据播种配置。
/// </summary>
public sealed class AuthCenterSeedOptions
{
    /// <summary>
    /// 配置节名称。
    /// </summary>
    public const string SectionName = "Seed";

    /// <summary>
    /// 默认管理员配置。
    /// </summary>
    public AdminUserSeedOptions Admin { get; init; } = new();

    /// <summary>
    /// 预置客户端配置集合。
    /// </summary>
    public List<AuthCenterClientSeedOptions> Clients { get; init; } = [];
}

/// <summary>
/// 默认管理员账户播种配置。
/// </summary>
public sealed class AdminUserSeedOptions
{
    public string UserName { get; init; } = "admin";

    public string Email { get; init; } = "admin@eztrade.local";

    public string Password { get; init; } = "Admin123!";
}

/// <summary>
/// OpenIddict 客户端播种配置。
/// </summary>
public sealed class AuthCenterClientSeedOptions
{
    public string ClientId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string ClientType { get; init; } = "public";

    public string? ClientSecret { get; init; }

    public List<string> GrantTypes { get; init; } = [];

    public List<string> AllowedScopes { get; init; } = [];

    public List<string> GrantedPermissions { get; init; } = [];
}
