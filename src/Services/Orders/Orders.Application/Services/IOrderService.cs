using Orders.Application.Commands;
using Orders.Application.Search;
using Orders.Domain.Entities;

namespace Orders.Application.Services;

public interface IOrderService
{
    Task<Guid> PlaceAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default);

    Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default);
}
