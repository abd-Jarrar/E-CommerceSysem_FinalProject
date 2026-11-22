using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed record PriceResult(decimal FinalPrice,decimal DiscountAmount,string AppliedRule)
    {
        public static PriceResult NoDiscount(decimal basePrice, string rule = "Normal")
            => new(basePrice, 0m, rule);

        public static PriceResult FromPercentage(decimal basePrice, decimal percentage, string rule)
        {
            var discount = Math.Round(basePrice * percentage, 2, MidpointRounding.AwayFromZero);
            return new PriceResult(basePrice - discount, discount, rule);
        }
    }
}
