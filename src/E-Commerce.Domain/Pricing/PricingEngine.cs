using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed class PricingEngine
    {
        private readonly IPricingStrategy[] _strategies;

        public PricingEngine(IEnumerable<IPricingStrategy> strategies)
        {
            _strategies = strategies.ToArray();
        }

        public PriceResult Calculate(PricingContext context)
        {
            var best = _strategies
                .Where(s => s.AppliesTo(context))
                .Select(s => Guard(s.Calculate(context), context.BasePrice))
                .MinBy(r => r.FinalPrice);

            return best ?? PriceResult.NoDiscount(context.BasePrice);
        }

        private static PriceResult Guard(PriceResult result, decimal basePrice)
        {
            var final = Math.Clamp(result.FinalPrice, 0m, basePrice);
            return result with { FinalPrice = final, DiscountAmount = basePrice - final };
        }
    }
}
