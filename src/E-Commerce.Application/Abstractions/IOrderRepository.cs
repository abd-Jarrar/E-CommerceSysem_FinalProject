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
        
    }
}
