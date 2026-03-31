namespace AuthCenter.Api.Services;

public interface IAuthCenterDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
