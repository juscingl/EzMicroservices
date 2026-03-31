namespace Orders.Application.Search;

public interface IOrderSearchReader
{
    Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default);
}
