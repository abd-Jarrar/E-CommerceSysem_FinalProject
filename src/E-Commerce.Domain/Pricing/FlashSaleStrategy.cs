using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Pricing
{
    public sealed class FlashSaleStrategy : IPricingStrategy
    {
        private readonly DateTime _startUtc;
        private readonly DateTime _endUtc;
        private readonly decimal _percentage;
        private readonly HashSet<Guid> _productIds;

        public FlashSaleStrategy(DateTime startUtc, DateTime endUtc, decimal percentage, IEnumerable<Guid> productIds)
        {
            if (productIds is null)
                throw new ArgumentNullException(nameof(productIds), "Flash sale needs a product list.");
            if (endUtc <= startUtc)
                throw new ArgumentException("End must be after start.", nameof(endUtc));
            if (percentage is <= 0m or >= 1m)
                throw new ArgumentOutOfRangeException(nameof(percentage));
            _startUtc = startUtc;
            _endUtc = endUtc;
            _percentage = percentage;
            _productIds = productIds.ToHashSet();
        }

        public bool AppliesTo(PricingContext context)
            => context.AtUtc >= _startUtc
               && context.AtUtc < _endUtc
               && _productIds.Contains(context.ProductId);

        public PriceResult Calculate(PricingContext context)
            => PriceResult.FromPercentage(context.BasePrice, _percentage, "FlashSale");
    }
}
