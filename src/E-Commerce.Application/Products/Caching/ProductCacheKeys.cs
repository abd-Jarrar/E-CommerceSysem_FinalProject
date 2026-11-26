using E_Commerce.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Caching
{
    public static class ProductCacheKeys
    {
        private const string _productCacheTag = "product";
        public const string Tag = _productCacheTag; 
                
        extension(Product product)
        {
            public string ToCacheKey()
            {
                return $"{_productCacheTag}:{product.Id}";
            }
        }
        public static string GetCacheKey(Guid guid)
        {
            return $"{_productCacheTag}:{guid}";
        }
    }
}
