namespace BuildingBlocks.Exceptions;

public sealed class EntityNotFoundException(Type entityType, object? key)
    : InvalidOperationException($"{entityType.Name} with key '{key}' was not found.");
