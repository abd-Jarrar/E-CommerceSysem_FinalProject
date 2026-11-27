using E_Commerce.Application.Orders.Dto;
using E_Commerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Mappers
{
    public static class OrderItemMapper
    {
        extension(OrderItem item)
        {
            public OrderItemDto ToDto() => new(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.LineTotal);
        }
        extension(IEnumerable<OrderItem> items)
        {
            public List<OrderItemDto> ToDtos()
            {
                return items.Select(i => i.ToDto()).ToList();
            }
        }
    }
}
