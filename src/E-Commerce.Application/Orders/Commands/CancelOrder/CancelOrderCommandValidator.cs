using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandValidator:AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order id is required.");
        }
    }
}
