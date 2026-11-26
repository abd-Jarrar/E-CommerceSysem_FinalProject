using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator:AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("The price should be bigger than 0.00");
            RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("Product id is required.");
        }
    }
}
