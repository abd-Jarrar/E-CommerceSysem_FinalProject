using E_Commerce.Application.Orders.Dto;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Queries
{
    public sealed record GetOrderQuery(Guid Id) : IRequest<Result<OrderDto>>;
}
