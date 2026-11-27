using E_Commerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Dto
{
    public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);
}
