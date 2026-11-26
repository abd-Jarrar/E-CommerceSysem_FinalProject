using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Caching
{
    public static class CustomerCacheKeys
    {
        private const string _customerCacheTag = "customer";
        public const string Tag = _customerCacheTag;

        extension(Customer customer)
        {
            public string ToCacheKey()
            {
                return $"{_customerCacheTag}:{customer.Id}";
            }
        }
        public static string ToCacheKey(Guid guid)
        {
            return $"{_customerCacheTag}:{guid}";
        }
    }
}
