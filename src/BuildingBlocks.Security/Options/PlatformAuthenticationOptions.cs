namespace BuildingBlocks.Security.Options;

public sealed class PlatformAuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Issuer { get; init; } = "http://localhost:5000/";

    public string Authority { get; init; } = "http://localhost:5000/";

    public string Audience { get; init; } = "eztrade-platform";

    public int AccessTokenExpirationMinutes { get; init; } = 60;

    public int RefreshTokenExpirationDays { get; init; } = 14;
}
