using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed class SeasonalDiscountStrategy : IPricingStrategy
    {
        private readonly DateTime _startUtc;
        private readonly DateTime _endUtc;
        private readonly decimal _percentage;

        public SeasonalDiscountStrategy(DateTime startUtc, DateTime endUtc, decimal percentage)
        {
            if (endUtc <= startUtc)
                throw new ArgumentException("End must be after start.", nameof(endUtc));
            if (percentage is <= 0m or >= 1m)
                throw new ArgumentOutOfRangeException(nameof(percentage));

            _startUtc = startUtc;
            _endUtc = endUtc;
            _percentage = percentage;
        }

        public bool AppliesTo(PricingContext context)
            => context.AtUtc >= _startUtc && context.AtUtc < _endUtc;

        public PriceResult Calculate(PricingContext context)
            => PriceResult.FromPercentage(context.BasePrice, _percentage, "Seasonal");
    }
}
