using E_Commerce.Application.Orders.Dto;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<OrderLine> Items)
        : IRequest<Result<OrderDto>>;
}
