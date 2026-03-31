using AuthCenter.Api.Options;

namespace AuthCenter.Api.Services;

public interface ISeededClientRegistry
{
    IReadOnlyCollection<AuthCenterClientSeedOptions> GetAll();

    AuthCenterClientSeedOptions? Find(string clientId);
}
