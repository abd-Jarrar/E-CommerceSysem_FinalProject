using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Commands.UpdateCustomer;
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

namespace E_Commerce.Application.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository productRepository , ILogger<UpdateProductCommandHandler> logger, HybridCache cache)
        : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<UpdateProductCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private const string _productCacheTag = "product";


        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                _logger.LogInformation("Product update rejected: product {ProductId} not found.", request.Id);
                return ProductErrors.NotFound(request.Id);
            }

            var updated = product.Update(request.Name, request.Price);
            if (updated.IsError)
                return updated.Errors;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                await _cache.RemoveByTagAsync(_productCacheTag, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for product {ProductId}.", request.Id);
            }

            _logger.LogInformation("Product {ProductId} updated.", request.Id);
            return product.ToDto();
        }
    }
}
