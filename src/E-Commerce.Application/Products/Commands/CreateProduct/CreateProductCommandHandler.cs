using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Application.Products.Mapper;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace E_Commerce.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler(IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        ILogger<CreateProductCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private const string _productCacheTag= "product"; 
        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var sku= (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            if (await _productRepository.SkuExistsAsync(sku, cancellationToken))
            {
                _logger.LogWarning("Product creation aborted. SKU already exists.");
                return ProductErrors.SKUExists;
            }
            var createProductResult = Product.Create(sku, request.Name.Trim(), request.Price, request
                .Stock);
            if(createProductResult.IsError)
                return createProductResult.Errors; 
            await _productRepository.AddProductAsync(createProductResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var product = createProductResult.Value;
            _logger.LogInformation("Product created successfully. Id: {product.Id}", product.Id);
            try
            {
                await _cache.RemoveByTagAsync(_productCacheTag, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for the products");
            }
            return product.ToDto();

        }
    }
}
