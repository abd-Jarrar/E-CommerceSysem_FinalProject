using E_Commerce.Application.Orders.Queries.SearchOrder;
using E_Commerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Abstractions
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Order order, CancellationToken ct);
        Task<(IReadOnlyList<Order> Items, int TotalCount)> SearchAsync(
        SearchOrdersQuery query, CancellationToken ct);

    }
}
