using Orders.Domain.Entities;

namespace Orders.Application.Search;

public interface IOrderSearchIndexer
{
    Task IndexAsync(Order order, CancellationToken cancellationToken = default);
}
