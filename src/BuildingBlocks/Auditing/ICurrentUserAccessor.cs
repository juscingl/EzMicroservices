namespace BuildingBlocks.Auditing;

public interface ICurrentUserAccessor
{
    Guid? UserId { get; }

    string? UserName { get; }
}
