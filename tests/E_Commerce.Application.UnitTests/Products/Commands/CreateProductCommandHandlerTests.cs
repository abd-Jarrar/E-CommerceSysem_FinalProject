using Autofac.Extras.Moq;
using Castle.Core.Logging;
using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Commands.CreateCustomer;
using E_Commerce.Application.Products.Commands.CreateProduct;
using E_Commerce.Domain.Products;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.UnitTests.Products.Commands
{
    public class CreateProductCommandHandlerTests
    {
        private readonly CancellationToken ct = CancellationToken.None;

        [Fact]
        public async Task CreateProductCommandHandler_SKUExists_SkuExistsError()
        {
            //arrange
            var request = new CreateProductCommand("product", "sku", 20, 20.3m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku,ct)).ReturnsAsync(true);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.SKUExists,result.Errors);
        }

        [Fact]
        public async Task CreateProductCommandHandler_NullSku_SKURequired()
        {
            //arrange
            var request = new CreateProductCommand("product", null!, 20, 20.3m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.SkuRequired, result.Errors);
        }

        [Fact]
        public async Task CreateProductCommandHandler_NegativePrice_NegativeOrZeroPrice()
        {
            //arrange
            var request = new CreateProductCommand("product","sku", 20, -20.3m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NegativeOrZeroPrice, result.Errors);
        }

        [Fact]
        public async Task CreateProductCommandHandler_ZeroPrice_NegativeOrZeroPrice()
        {
            //arrange
            var request = new CreateProductCommand("product", "sku", 20, 0m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NegativeOrZeroPrice, result.Errors);
        }

        [Fact]
        public async Task CreateProductCommandHandler_NegativeStock_NegativeStock()
        {
            //arrange
            var request = new CreateProductCommand("product", "sku", -2, 20m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NegativeStock, result.Errors);
        }

        [Fact]
        public async Task CreateProductCommandHandler_ValidDetails_ProductDto()
        {
            //arrange
            var request = new CreateProductCommand("product", "sku", 10, 20m);
            var sku = (request.Sku ?? string.Empty).Trim().ToLowerInvariant();
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.SkuExistsAsync(sku, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(request.Name, result.Value.Name);
            Assert.Equal(request.Price, result.Value.Price);
            Assert.Equal(request.Stock, result.Value.Quantity);
            mock.Mock<IProductRepository>().Verify(x => x.AddProductAsync(It.IsAny<Product>(), ct),Times.Once);
            mock.Mock<IUnitOfWork>().Verify(x=>x.SaveChangesAsync(ct), Times.Once);
        }

    }
}
