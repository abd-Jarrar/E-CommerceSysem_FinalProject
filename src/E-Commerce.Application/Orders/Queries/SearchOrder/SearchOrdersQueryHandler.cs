using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Common;
using E_Commerce.Application.Orders.Dto;
using E_Commerce.Application.Orders.Mappers;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Queries.SearchOrder
{
    public sealed class SearchOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<SearchOrdersQuery, Result<PaginatedList<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository = orderRepository;

        public async Task<Result<PaginatedList<OrderDto>>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _orderRepository.SearchAsync(request, cancellationToken);

            return new PaginatedList<OrderDto>(
                items.ToDtos(), request.Page, request.PageSize, total);
        }
    }
}
