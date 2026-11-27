using E_Commerce.Application.Common;
using E_Commerce.Application.Products.Queries.SearchProducts;
using E_Commerce.Domain.Products;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace E_Commerce.Application.Abstractions
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id,CancellationToken ct);
        Task AddProductAsync(Product product,CancellationToken ct);
        Task<bool> SkuExistsAsync(string sku, CancellationToken ct);
        Task<bool> IncreaseStAsync(Guid id,int quantity,CancellationToken ct);
        Task<bool> TryDecreasAsync(Guid id,int quantity,CancellationToken ct);
        Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
    SearchProductsQuery query, CancellationToken ct);

    }
}
