using E_Commerce.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Tests
{
    public class OrderTests
    {
        private static Order CreateOrderWithItem()
        {
            var order = Order.Create(Guid.NewGuid()).Value;
            order.AddItem(Guid.NewGuid(), "Phone", 1, 100m);
            return order;
        }
        [Fact]
        public void Create_ValidCustomerId_ReturnsPendingOrder()
        {
            //Arrange
            var customerId = Guid.NewGuid();
            //Act
            var result = Order.Create(customerId);
            //Assert
            Assert.True(result.IsSuccess);
            var order = result.Value;
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Empty(order.Items);
            Assert.NotEqual(Guid.Empty, order.Id);
        }
        [Fact]
        public void Create_EmptyCustomerId_ReturnsCustomerIdRequired()
        {
            // Act
            var result = Order.Create(Guid.Empty);
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(OrderErrors.CustomerIdRequired, result.TopError);
        }
        [Fact]
        public void Create_CalledTwice_GeneratesDifferentIds()
        {
            //Arrange
            var customerId = Guid.NewGuid();
            //Act
            var first = Order.Create(customerId).Value;
            var second = Order.Create(customerId).Value;
            //Assert
            Assert.NotEqual(first.Id, second.Id);
        }
        [Fact]
        public void Cancel_PendingOrder_SetsStatusToCancelled()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid()).Value;
            //Act
            var result = order.Cancel();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
        [Fact]
        public void Cancel_AlreadyCancelledOrder_ReturnsAlreadyCancelled()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid()).Value;
            // Act
             order.Cancel();
            var result= order.Cancel();
            // Assert
            Assert.True(result.IsError);
            Assert.Equal(OrderErrors.AlreadyCancelled, result.TopError);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
        [Fact]
        public void Cancel_CompletedOrder_ReturnsCannotCancelCompleted()
        {
            //Arrange
            var order = CreateOrderWithItem();
            //Act
            order.Complete();
            var result = order.Cancel();
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(OrderErrors.CannotCancelCompleted, result.TopError);
            Assert.Equal(OrderStatus.Completed, order.Status);
        }
        [Fact]
        public void Complete_PendingOrderWithItems_SetsStatusToCompleted()
        {
            //Arrange
            var order = CreateOrderWithItem();
            //Act
            var result = order.Complete();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(OrderStatus.Completed, order.Status);
        }
        [Fact]
        public void Complete_EmptyOrder_ReturnsEmpty()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid()).Value;
            //Act
            var result = order.Complete();
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(OrderErrors.Empty, result.TopError);
            Assert.Equal(OrderStatus.Pending, order.Status);
        }
        [Fact]
        public void Complete_AlreadyCompletedOrder_ReturnsNotPending()
        {
            //Arrange
            var order = CreateOrderWithItem();
            order.Complete();

            //Act
            var result = order.Complete();

            //Assert
            Assert.Equal(OrderErrors.NotPending, result.TopError);
            Assert.Equal(OrderStatus.Completed, order.Status);
        }
        [Fact]
        public void Complete_CancelledOrder_ReturnsNotPending()
        {
            //Arrange
            var order = CreateOrderWithItem();
            order.Cancel();
            //Act
            var result = order.Complete();
            //Assert
            Assert.Equal(OrderErrors.NotPending, result.TopError);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }
     
        [Fact]
        public void AddItem_ValidItem_AddsItemToOrder()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            // Act
            var result = order.AddItem(productId, "  Phone  ", 2, 50m);
            // Assert
            Assert.True(result.IsSuccess);
            var item = Assert.Single(order.Items);
            Assert.Equal(order.Id, item.OrderId);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal("Phone", item.ProductName);   
            Assert.Equal(2, item.Quantity);
            Assert.Equal(50m, item.UnitPrice);
            Assert.Equal(100m, order.TotalAmount);
        }

        [Fact]
        public void AddItem_ZeroPrice_IsAccepted()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            //Act
            var result = order.AddItem(Guid.NewGuid(), "Free gift", 1, 0m);
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Single(order.Items);
        }

        [Fact]
        public void AddItem_CompletedOrder_ReturnsNotPendingAndAddsNothing()
        {
            //Arrange
            var order = CreateOrderWithItem();
            order.Complete();
            // Act
            var result = order.AddItem(Guid.NewGuid(), "Case", 1, 10m);
            // Assert
            Assert.Equal(OrderErrors.NotPending, result.TopError);
            Assert.Single(order.Items);
        }

        [Fact]
        public void AddItem_CancelledOrder_ReturnsNotPending()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            order.Cancel();
            //Act
            var result = order.AddItem(Guid.NewGuid(), "Phone", 1, 100m);
            //Assert
            Assert.Equal(OrderErrors.NotPending, result.TopError);
            Assert.Empty(order.Items);
        }

        [Fact]
        public void AddItem_DuplicateProduct_ReturnsDuplicateProduct()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            Guid productId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            order.AddItem(productId, "Phone", 1, 100m);

            // Act
            var result = order.AddItem(productId, "Phone", 2, 100m);

            // Assert
            Assert.Equal(OrderErrors.DuplicateProduct, result.TopError);
            Assert.Single(order.Items);
        }

        [Fact]
        public void AddItem_AllInputsInvalid_ReturnsThreeErrors()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            //Act
            var result = order.AddItem(Guid.NewGuid(), " ", 0, -1m);
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains(OrderErrors.ProductNameRequired, result.Errors);
            Assert.Contains(OrderErrors.InvalidQuantity, result.Errors);
            Assert.Contains(OrderErrors.InvalidUnitPrice, result.Errors);
            Assert.Empty(order.Items);
        }

        [Fact]
        public void AddItem_NonPositiveQuantity_ReturnsInvalidQuantity()
        {
            //Arrange
            Guid customerId = Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            //Act
            var result = order.AddItem(Guid.NewGuid(), "Phone", -10, 100m);

            Assert.Equal(OrderErrors.InvalidQuantity, result.TopError);
            Assert.Empty(order.Items);
        }

        [Fact]
        public void AddItem_NegativePrice_ReturnsInvalidUnitPrice()
        {
            //Arrange
            Guid customerId= Guid.NewGuid();
            var order = Order.Create(customerId).Value;
            //Act
            var result = order.AddItem(Guid.NewGuid(), "Phone", 1, -0.01m);
            //Assert
            Assert.Equal(OrderErrors.InvalidUnitPrice, result.TopError);
            Assert.Empty(order.Items);
        }
    }
}
