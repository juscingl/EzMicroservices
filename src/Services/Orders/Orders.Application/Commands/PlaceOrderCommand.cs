using Orders.Application.Dtos;

namespace Orders.Application.Commands;

public sealed record PlaceOrderCommand(Guid CustomerId, IReadOnlyCollection<OrderLineDto> Lines);
