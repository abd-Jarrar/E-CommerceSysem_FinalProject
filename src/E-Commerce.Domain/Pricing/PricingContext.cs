using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed record PricingContext(Guid ProductId,
    decimal BasePrice,
    bool IsVipCustomer,
    DateTime AtUtc);
}
