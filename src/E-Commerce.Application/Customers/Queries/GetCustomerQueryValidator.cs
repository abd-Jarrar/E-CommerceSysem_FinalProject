using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Queries
{
    public sealed class GetCustomerQueryValidator:AbstractValidator<GetCustomerQuery>
    {
        public GetCustomerQueryValidator()
        {
            RuleFor(c => c.Id)
             .NotEmpty()
             .WithMessage("Customer id is required.");
        }
    }
}
