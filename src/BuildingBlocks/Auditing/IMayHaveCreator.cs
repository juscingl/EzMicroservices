namespace BuildingBlocks.Auditing;

public interface IMayHaveCreator
{
    Guid? CreatorId { get; set; }
}
