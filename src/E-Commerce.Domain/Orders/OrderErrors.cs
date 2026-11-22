using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Orders
{
    public static class OrderErrors
    {
        public static Error CustomerIdRequired =>
            Error.Validation("Order_Customer_Id_Required", "Customer id is required");
        public static Error NotFound(Guid id) =>
            Error.NotFound("Order_Not_Found", $"Order with the id \"{id}\" not found");
        public static Error Empty =>
            Error.Validation("Order_Is_Empty", "the order has no items");
        public static Error NotPending =>
            Error.Conflict("Order_Status_Not_Pending", "the order status is not pending");
        public static Error AlreadyCancelled =>
            Error.Conflict("Order_Is_Already_Cancelled", "the order is already cancelled");
        public static Error CannotCancelCompleted =>
            Error.Conflict("Order_Is_Completed", "Can't cancel completed order");
        public static Error ProductNameRequired =>
            Error.Validation("Order_Product_Name_Is_Required", "Product name is required");
        public static Error InvalidQuantity =>
            Error.Validation("Order_Quantity_MustBePositive", "Quantity must be greater than zero.");
        public static Error InvalidUnitPrice =>
            Error.Validation("Order_UnitPrice_MustBeNonNegative", "Unit price must not be negative.");
        public static Error DuplicateProduct =>
            Error.Conflict("Order_Product_Is_Duplicated", "The Product Is Already In the order.");
        
    }
}
