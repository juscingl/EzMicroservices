namespace BuildingBlocks.Search.Abstractions;

public interface IIndexNameResolver
{
    string Resolve(string logicalName);
}
