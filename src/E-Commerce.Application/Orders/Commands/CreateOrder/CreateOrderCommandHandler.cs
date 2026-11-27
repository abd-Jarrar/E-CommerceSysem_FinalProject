using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Orders.Dto;
using E_Commerce.Application.Orders.Mappers;
using E_Commerce.Application.Products.Caching;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Orders;
using E_Commerce.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateOrderCommandHandler> logger, IOrderRepository orderRepository,
   IProductRepository productRepository, ICustomerRepository customerRepository, HybridCache cache) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken ct)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
            if (customer is null)
            {
                _logger.LogInformation("Order rejected: customer {CustomerId} not found.", request.CustomerId);
                return CustomerErrors.NotFound(request.CustomerId);
            }

            var orderResult = Order.Create(request.CustomerId);
            if (orderResult.IsError)
                return orderResult.Errors;
            var order = orderResult.Value;

            foreach (var line in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(line.ProductId, ct);
                if (product is null)
                {
                    _logger.LogInformation("Order rejected: product {ProductId} not found.", line.ProductId);
                    return ProductErrors.NotFound(line.ProductId);
                }

                if (!product.IsActive)
                {
                    _logger.LogInformation("Order rejected: product {ProductId} is inactive.", product.Id);
                    return ProductErrors.InActive;
                }

                var added = order.AddItem(product.Id, product.Name, line.Quantity, product.Price);
                if (added.IsError)
                    return added.Errors;
            }

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                foreach (var item in order.Items.OrderBy(i => i.ProductId))
                {
                    if (!await _productRepository.TryDecreasAsync(item.ProductId, item.Quantity, ct))
                    {
                        await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                        _logger.LogInformation(
                            "Order rejected: insufficient stock for product {ProductId}. Requested {Quantity}.",
                            item.ProductId, item.Quantity);
                        return ProductErrors.InsufficientStock(item.ProductId);
                    }
                }

                await _orderRepository.AddAsync(order, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }

            _logger.LogInformation(
                "Order {OrderId} created for customer {CustomerId} with {ItemCount} item(s).",
                order.Id, order.CustomerId, order.Items.Count);

            try
            {
                foreach (var item in order.Items)
                    await _cache.RemoveAsync(ProductCacheKeys.ToCacheKey(item.ProductId), ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for order {OrderId}.", order.Id);
            }

            return order.ToDto();
        }
    }
}
