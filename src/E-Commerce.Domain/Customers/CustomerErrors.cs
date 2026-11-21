using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Customers
{
    public static class CustomerErrors
    {
        public static Error NameRequired =>
            Error.Validation("Customer_Name_Required", "Cusomter name is required");
        public static Error EmailRequired =>
            Error.Validation("Customer_Email_Required", "Cusomter Email is required");
        public static Error EmailInvalid =>
            Error.Validation("Customer_Email_Invalid", "Customer email is not in a valid format.");

        public static Error NotFound(Guid id) =>
            Error.NotFound("Customer_NotFound", $"Customer with id \"{id}\" was not found.");

        public static Error EmailExists =>
            Error.Conflict("Customer_Email_Exists", "A customer with this email already exists.");
    }
}
