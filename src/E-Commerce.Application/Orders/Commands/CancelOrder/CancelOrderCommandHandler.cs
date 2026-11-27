using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Caching;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Orders;
using E_Commerce.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler(
    IOrderRepository orders,
    IProductRepository products,
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<CancelOrderCommandHandler> logger)
    : IRequestHandler<CancelOrderCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(CancelOrderCommand request, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(request.OrderId, ct);
            if (order is null)
            {
                logger.LogInformation("Order cancel rejected: order {OrderId} not found.", request.OrderId);
                return OrderErrors.NotFound(request.OrderId);
            }

            var cancelled = order.Cancel();
            if (cancelled.IsError)
            {
                logger.LogInformation(
                    "Order cancel rejected for {OrderId}: {ErrorCode}.",
                    order.Id, cancelled.Errors[0].Code);
                return cancelled.Errors;
            }

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                foreach (var item in order.Items.OrderBy(i => i.ProductId))
                {
                    if (!await products.IncreaseStAsync(item.ProductId, item.Quantity, ct))
                    {
                        await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                        logger.LogWarning(
                            "Order cancel failed for {OrderId}: could not restore stock for product {ProductId}.",
                            order.Id, item.ProductId);
                        return ProductErrors.StockRestoreFailed(item.ProductId);
                    }
                }
                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitTransactionAsync(ct);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                throw;
            }

            logger.LogInformation(
                "Order {OrderId} cancelled. Stock restored for {ItemCount} item(s).",
                order.Id, order.Items.Count);

            try
            {
                foreach (var item in order.Items)
                    await cache.RemoveAsync(ProductCacheKeys.ToCacheKey(item.ProductId), ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cache invalidation failed for order {OrderId}.", order.Id);
            }

            return Result.Updated;
        }
    }
}
