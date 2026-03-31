namespace BuildingBlocks.Auditing;

public interface IHasModificationTime
{
    DateTimeOffset? LastModificationTime { get; set; }
}
