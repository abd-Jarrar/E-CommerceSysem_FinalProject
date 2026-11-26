using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Commands.IncreaseStock
{
    public class IncreaseStockCommandValidator:AbstractValidator<IncreaseStockCommand>
    {
        public IncreaseStockCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product id is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("The quantity shouldn't be negative");
        }
    }
}
