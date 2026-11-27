using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator:AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
           .NotEmpty()
           .WithMessage("Customer id is required.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("The order must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty()
                    .WithMessage("Product id is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero.");
            });

            RuleFor(x => x.Items)
                .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
                .WithMessage("Each product can appear only once.")
                .When(x => x.Items is { Count: > 0 });
        }
    }
}
