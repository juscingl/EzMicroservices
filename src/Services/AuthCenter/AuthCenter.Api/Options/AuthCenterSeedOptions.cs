namespace AuthCenter.Api.Options;

public sealed class AuthCenterSeedOptions
{
    public const string SectionName = "Seed";

    public AdminUserSeedOptions Admin { get; init; } = new();

    public List<AuthCenterClientSeedOptions> Clients { get; init; } = [];
}

public sealed class AdminUserSeedOptions
{
    public string UserName { get; init; } = "admin";

    public string Email { get; init; } = "admin@eztrade.local";

    public string Password { get; init; } = "Admin123!";
}

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
