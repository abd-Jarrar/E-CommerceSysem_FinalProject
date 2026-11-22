using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Domain.Orders
{
    public sealed class Order : Entity
    {
        private readonly List<OrderItem> _items = [];

        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalAmount => _items.Sum(i => i.LineTotal);

        private Order() { }
        private Order(Guid id, Guid customerId)
    : base(id)
        {
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public static Result<Order> Create(Guid customerId)
        {
            if (customerId == Guid.Empty)
                return OrderErrors.CustomerIdRequired;

            return new Order(Guid.NewGuid(), customerId);
        }
        public Result<Updated> AddItem(Guid productId, string productName, int quantity, decimal unitPrice) {
            if (Status != OrderStatus.Pending)
                return OrderErrors.NotPending;
            if (_items.Any(i => i.ProductId == productId))
                return OrderErrors.DuplicateProduct;
            var errors = new List<Error>();
            if (string.IsNullOrWhiteSpace(productName))
                errors.Add(OrderErrors.ProductNameRequired);
            if (quantity <= 0)
                errors.Add(OrderErrors.InvalidQuantity);
            if (unitPrice < 0)
                errors.Add(OrderErrors.InvalidUnitPrice);
            if (errors.Count > 0)
                return errors;
            _items.Add(new OrderItem(Id, productId, productName.Trim(), quantity, unitPrice));
            return Result.Updated;
        }
        public Result<Updated> Complete() {
            if (Status is not OrderStatus.Pending)
                return OrderErrors.NotPending;
            if (_items.Count == 0)
                return OrderErrors.Empty;
            Status = OrderStatus.Completed;
            return Result.Updated;
        }
        public Result<Updated> Cancel() {
            if (Status is OrderStatus.Cancelled)
                return OrderErrors.AlreadyCancelled;
            if (Status is OrderStatus.Completed)
                return OrderErrors.CannotCancelCompleted;
            Status = OrderStatus.Cancelled;
            return Result.Updated;
        }
    }
}
