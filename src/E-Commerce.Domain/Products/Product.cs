using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Products
{
    public sealed class Product :Entity
    {
        public string Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public decimal Price { get; private set; }
        public int AvailableStock { get; private set; }
        public bool IsActive { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;

        private Product() { }   

        private Product(Guid id, string sku, string name, decimal price, int stock)
        : base(id)
        {
           
            Sku= sku;
            Name= name;
            Price= price;
            AvailableStock = stock;
            IsActive = true;
        }

        public static Result<Product> Create(string sku, string name, decimal price, int stock)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(sku))
                errors.Add(ProductErrors.SkuRequired);
            if (string.IsNullOrWhiteSpace(name))
                errors.Add(ProductErrors.NameRequired);
            if (price <= 0)
                errors.Add(ProductErrors.NegativeOrZeroPrice);
            if (stock < 0)
                errors.Add(ProductErrors.NegativeStock);
            if (errors.Count > 0)
                return errors;
            
            return new Product(Guid.NewGuid(), sku.Trim(), name.Trim(), price, stock);
        }
    }
}
