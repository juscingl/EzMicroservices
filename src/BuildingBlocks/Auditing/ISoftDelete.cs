namespace BuildingBlocks.Auditing;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
