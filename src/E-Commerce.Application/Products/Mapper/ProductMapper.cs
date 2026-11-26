using E_Commerce.Application.Customers.Dtos;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Mapper
{
    public static class ProductMapper
    {
        extension(Product product)
        {
            public ProductDto ToDto()
            {
                ArgumentNullException.ThrowIfNull(product);
                return new ProductDto(product.Id, product.Name, product.Sku,
                    product.AvailableStock,product.Price,product.IsActive);
            }
        }
        extension(IEnumerable<Product> products)
        {
            public List<ProductDto> ToDtos()
            {
                return products.Select(c => c.ToDto()).ToList();
            }
        }
    }
}
