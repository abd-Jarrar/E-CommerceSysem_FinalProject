using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed class NormalPriceStrategy : IPricingStrategy
    {
        public bool AppliesTo(PricingContext context) => true;

        public PriceResult Calculate(PricingContext context)
            => PriceResult.NoDiscount(context.BasePrice);
    }
}
