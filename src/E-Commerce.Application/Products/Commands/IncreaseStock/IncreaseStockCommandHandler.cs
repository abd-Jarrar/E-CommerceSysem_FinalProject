using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Caching;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Application.Products.Mapper;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.IncreaseStock
{
    public sealed class IncreaseStockCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository,
        ILogger<IncreaseStockCommandHandler> logger,
        HybridCache cache) : IRequestHandler<IncreaseStockCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<IncreaseStockCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<ProductDto>> Handle(IncreaseStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                _logger.LogInformation("Product update rejected: product {ProductId} not found.", request.Id);
                return ProductErrors.NotFound(request.Id);
            }
            var updated = product.IncreaseStock(request.Quantity);
            if (updated.IsError)
                return updated.Errors;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            try
            {
                await _cache.RemoveAsync(product.ToCacheKey(), cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for product {ProductId}.", request.Id);
            }
            _logger.LogInformation("Product {ProductId}  was increased by {} .", request.Id,request.Quantity);
            return product.ToDto();

        }
    }
}
