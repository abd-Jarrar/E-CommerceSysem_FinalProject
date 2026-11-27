using E_Commerce.Application.Common;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Queries.SearchProducts
{
    public enum ProductSortField { Name = 1, Price = 2, Stock = 3 }
    public enum SortDirection { Ascending = 1, Descending = 2 }
    public sealed record SearchProductsQuery(
    string? SearchText = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = true,
    ProductSortField SortBy = ProductSortField.Name,
    SortDirection Direction = SortDirection.Ascending,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<ProductDto>>>;
}
