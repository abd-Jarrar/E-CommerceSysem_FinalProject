using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Orders.Queries.SearchOrder
{
    public sealed class SearchOrdersQueryValidator:AbstractValidator<SearchOrdersQuery>
    {
        public SearchOrdersQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.CustomerId)
                .NotEqual(Guid.Empty)
                .When(x => x.CustomerId.HasValue);
            RuleFor(x => x)
                .Must(x => x.FromUtc is null || x.ToUtc is null || x.FromUtc <= x.ToUtc)
                .WithMessage("FromUtc must be earlier than or equal to ToUtc.");
        }
    }
}
