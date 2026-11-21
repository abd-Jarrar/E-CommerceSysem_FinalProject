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

        private Product() { }   // For EF Core

        private Product(Guid id, string sku, string name, decimal price, int stock)
        : base(id)
        {
            Id= id;
            Sku= sku;
            Name= name;
            Price= price;
            AvailableStock = stock;
        }

        public static Result<Product> Create(string sku, string name, decimal price, int stock)
        {
            // TODO: validate, return Error.Validation(...) on failure
            // TODO: otherwise return the new product
        }
    }
}
