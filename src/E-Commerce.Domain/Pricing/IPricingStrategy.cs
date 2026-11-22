using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public interface IPricingStrategy
    {
        bool AppliesTo(PricingContext context);
        PriceResult Calculate(PricingContext context);
    }
}
