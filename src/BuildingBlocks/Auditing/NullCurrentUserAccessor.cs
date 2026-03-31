namespace BuildingBlocks.Auditing;

public sealed class NullCurrentUserAccessor : ICurrentUserAccessor
{
    public static NullCurrentUserAccessor Instance { get; } = new();

    public Guid? UserId => null;

    public string? UserName => null;

    private NullCurrentUserAccessor()
    {
    }
}
