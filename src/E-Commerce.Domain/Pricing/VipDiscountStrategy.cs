using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed class VipDiscountStrategy : IPricingStrategy
    {
        private readonly decimal _percentage;

        public VipDiscountStrategy(decimal percentage)
        {
            if (percentage is <= 0m or >= 1m)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Use a fraction between 0 and 1, e.g. 0.10.");
            _percentage = percentage;
        }

        public bool AppliesTo(PricingContext context) => context.IsVipCustomer;

        public PriceResult Calculate(PricingContext context)
            => PriceResult.FromPercentage(context.BasePrice, _percentage, "VIP");
    }
}
