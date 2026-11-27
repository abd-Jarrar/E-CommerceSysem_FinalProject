using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Common;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Application.Products.Mapper;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Queries.SearchProducts
{
    public sealed class SearchProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<SearchProductsQuery, Result<PaginatedList<ProductDto>>>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Result<PaginatedList<ProductDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _productRepository.SearchAsync(request, cancellationToken);

            return new PaginatedList<ProductDto>(
                items.Select(p => p.ToDto()).ToList(), request.Page, request.PageSize, total);
        }
    }
}
