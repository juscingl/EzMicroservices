namespace BuildingBlocks.Auditing;

public interface IHasCreationTime
{
    DateTimeOffset CreationTime { get; set; }
}
