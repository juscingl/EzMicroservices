namespace BuildingBlocks.Messaging.Registration;

public sealed class IntegrationConsumerRegistry
{
    private readonly List<IntegrationConsumerRegistration> _registrations = [];

    public IReadOnlyCollection<IntegrationConsumerRegistration> Registrations => _registrations.AsReadOnly();

    internal void Add(IntegrationConsumerRegistration registration)
    {
        _registrations.Add(registration);
    }
}
