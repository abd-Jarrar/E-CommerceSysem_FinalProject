using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Products.Queries.GetProduct
{
    public sealed class GetProductQueryValidator:AbstractValidator<GetProductQuery>
    {
        public GetProductQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product id is required.");
        }
    }
}
