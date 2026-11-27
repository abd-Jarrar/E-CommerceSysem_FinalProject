using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Products
{
    public static class ProductErrors
    {
        public static Error NameRequired =>
            Error.Validation("Product_Name_Required", "Product name is required");
        public static Error SkuRequired =>
            Error.Validation("Product_SKU_Required", "Product SKU is required");
        public static Error NegativeOrZeroPrice =>
            Error.Validation("Price_Must_Be_Positive", "Product price must be positive");
        public static Error NegativeStock =>
            Error.Validation("Stock_Shouldnt_Be_Negative", "Product stock should be zero or Positive");
        public static Error NotFound(Guid id) =>
            Error.NotFound("Product_Was_Not_Found", $"The Product With the Id \"{id}\" was not found ");
        public static Error SKUExists =>
            Error.Conflict("Product_Sku_Exists", "Product SKU Already exists");
        public static Error InActive =>
            Error.Validation("Product_Is_InActive", "Product is InActive");
        public static Error InsufficientStock(int requested, int available) =>
            Error.Conflict("Product_Stock_InSufficient", $"Product Available Stock is {available} while you requested {requested}");
        public static Error ConcurrencyConflict =>
            Error.Conflict("Product_Concurrency_Conflict",
                "The product was modified by another request. Reload it and try again.");
        public static Error InvalidQuantity =>
            Error.Validation("Product_Quantity_MustBePositive",
                "Quantity must be greater than zero.");
        public static Error StockRestoreFailed(Guid productId) =>
            Error.Failure("Product_Stock_Restore_Failed",
                $"Could not restore stock for product \"{productId}\".");
    }
}
