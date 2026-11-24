using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandValidator:AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Customer name is required.");
            RuleFor(c => c.Email).NotEmpty().EmailAddress().WithMessage("Customer email is invalid.");
        }
    }
}
