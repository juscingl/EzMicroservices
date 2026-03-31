using AuthCenter.Api.Options;
using Microsoft.Extensions.Options;

namespace AuthCenter.Api.Services;

public sealed class SeededClientRegistry(IOptions<AuthCenterSeedOptions> seedOptions) : ISeededClientRegistry
{
    private readonly AuthCenterSeedOptions _seedOptions = seedOptions.Value;

    public IReadOnlyCollection<AuthCenterClientSeedOptions> GetAll()
    {
        return _seedOptions.Clients;
    }

    public AuthCenterClientSeedOptions? Find(string clientId)
    {
        return _seedOptions.Clients.FirstOrDefault(client =>
            string.Equals(client.ClientId, clientId, StringComparison.OrdinalIgnoreCase));
    }
}
