using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Orders.Dto;
using E_Commerce.Application.Orders.Mappers;
using E_Commerce.Application.Products.Queries.GetProduct;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Queries
{
    public sealed class GetOrderQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderQuery, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

            return order is null
                ? OrderErrors.NotFound(request.Id)
                : order.ToDto();
        }
    }
}
