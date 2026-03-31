namespace Orders.Application.Dtos;

public sealed record OrderLineDto(Guid ProductId, int Quantity, decimal UnitPrice);
