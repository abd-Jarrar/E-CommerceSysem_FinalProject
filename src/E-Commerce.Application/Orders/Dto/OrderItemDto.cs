using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Dto
{
    public sealed record OrderItemDto(
     Guid ProductId,
     string ProductName,
     int Quantity,
     decimal UnitPrice,
     decimal TotalPrice);
}
