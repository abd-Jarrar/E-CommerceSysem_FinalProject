using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Dtos;
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

namespace E_Commerce.Application.Products.Commands.DeleteProduct
{
    public sealed class DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository
        , ILogger<DeleteProductCommandHandler> logger, HybridCache cache) : IRequestHandler<DeleteProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<DeleteProductCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<ProductDto>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
                return ProductErrors.NotFound(request.Id);
            product.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            try
            {
                await _cache.RemoveAsync(product.ToCacheKey(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for product {product.id}.", request.Id);
            }

            return product.ToDto();
        }
    }
}
