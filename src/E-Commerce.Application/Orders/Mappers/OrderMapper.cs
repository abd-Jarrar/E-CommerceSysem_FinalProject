using E_Commerce.Application.Orders.Dto;
using E_Commerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Mappers
{
    public static class OrderMapper
    {
        extension(Order order)
        {
            public OrderDto ToDto() => new(
                order.Id,
                order.CustomerId,
                order.Status,
                order.CreatedAtUtc,
                order.TotalAmount,
                order.Items.ToDtos());
        }

        extension(IEnumerable<Order> orders)
        {
            public List<OrderDto> ToDtos()
            {
                return orders.Select(o => o.ToDto()).ToList();
            }
        }
    }
}
