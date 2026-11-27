using E_Commerce.Application.Common;
using E_Commerce.Application.Orders.Dto;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Queries.SearchOrder
{
    public sealed record SearchOrdersQuery(
    Guid? CustomerId = null,
    OrderStatus? Status = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<OrderDto>>>;
}
