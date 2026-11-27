using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Caching;
using E_Commerce.Application.Products.Commands.DeleteProduct;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Application.Products.Mapper;
using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Products;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Application.Products.Queries.GetProduct
{
    public sealed class GetProductQueryHandler(
        IProductRepository productRepository
        , ILogger<GetProductQueryHandler> logger, HybridCache cache) : IRequestHandler<GetProductQuery, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<GetProductQueryHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var key = ProductCacheKeys.ToCacheKey(request.Id);
            var isMiss = false;

            try
            {
                var dto = await _cache.GetOrCreateAsync(
                    key,
                    async ct =>
                    {
                        isMiss = true;

                        var product = await _productRepository
                            .GetByIdAsync(request.Id, ct);

                        if (product is null)
                        {
                            throw new Exception("Product not found.");
                        }

                        return product.ToDto();
                    },
                    new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(5)
                    },
                    [ProductCacheKeys.Tag],
                    cancellationToken);

                if (isMiss)
                {
                    _logger.LogInformation("Cache MISS for {CacheKey}.", key);
                }
                else
                {
                    _logger.LogInformation("Cache HIT for {CacheKey}.", key);
                }

                return dto;
            }
            catch (Exception ex) when (ex.Message == "Product not found.")
            {
                _logger.LogInformation("Cache MISS for {CacheKey}.", key);
                return ProductErrors.NotFound(request.Id);
            }

        }
    }
}
