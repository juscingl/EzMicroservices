namespace BuildingBlocks.Security.Options;

/// <summary>
/// 表示PlatformAuthenticationOptions。
/// </summary>
public sealed class PlatformAuthenticationOptions
{
    /// <summary>
    /// 表示SectionName。
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    /// 获取或设置Issuer。
    /// </summary>
    public string Issuer { get; init; } = "http://localhost:5000/";

    /// <summary>
    /// 获取或设置Authority。
    /// </summary>
    public string Authority { get; init; } = "http://localhost:5000/";

    /// <summary>
    /// 获取或设置Audience。
    /// </summary>
    public string Audience { get; init; } = "eztrade-platform";

    /// <summary>
    /// 获取或设置AccessTokenExpirationMinutes。
    /// </summary>
    public int AccessTokenExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// 获取或设置RefreshTokenExpirationDays。
    /// </summary>
    public int RefreshTokenExpirationDays { get; init; } = 14;
}
