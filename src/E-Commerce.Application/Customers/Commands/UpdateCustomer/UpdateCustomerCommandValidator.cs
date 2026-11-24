using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Customers.Commands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandValidator:AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("Customer id is required.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Customer name is required.");
            RuleFor(c => c.Email).NotEmpty().EmailAddress().WithMessage("Customer email is invalid.");
        }
    }
}
