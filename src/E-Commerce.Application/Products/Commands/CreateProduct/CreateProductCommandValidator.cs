using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator:AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(c => c.Sku).NotEmpty().WithMessage("Product SKU is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("The price should be bigger than 0.00");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("The quantity shouldn't be negative");
        }
    }
}
