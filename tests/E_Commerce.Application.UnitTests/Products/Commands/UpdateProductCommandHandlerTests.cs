using Autofac.Extras.Moq;
using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Commands.CreateProduct;
using E_Commerce.Application.Products.Commands.IncreaseStock;
using E_Commerce.Application.Products.Commands.UpdateProduct;
using E_Commerce.Domain.Products;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.UnitTests.Products.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly CancellationToken ct = CancellationToken.None;

        [Fact]
        public async Task UpdateProductCommandHandler_NotFoundProduct_NotFound()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            var request = new UpdateProductCommand(product.Id,product.Name, 20m);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(default(Product));
            var hanlder = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await hanlder.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NotFound(product.Id), result.Errors);
        }
        [Fact]
        public async Task UpdateProductCommandHandler_NegativePrice_NegativeOrZeroPrice()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            var price = -20m;
            var request = new UpdateProductCommand(product.Id, product.Name, price);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);

            var handler = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NegativeOrZeroPrice, result.Errors);
        }

        [Fact]
        public async Task UpdateProductCommandHandler_ZeroPrice_NegativeOrZeroPrice()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            var newPrice = 0m;
            var request = new UpdateProductCommand(product.Id, product.Name, newPrice);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);

            var handler = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NegativeOrZeroPrice, result.Errors);
        }

        [Fact]
        public async Task UpdateProductCommandHandler_NullName_NameRequired()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            string? newName = null!;
            var request = new UpdateProductCommand(product.Id, newName, product.Price);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);

            var handler = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NameRequired, result.Errors);
        }

        [Fact]
        public async Task UpdateProductCommandHandler_ValidDetailsAndUpdateTheName_ProductDto()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            string? newName = "name";
            var request = new UpdateProductCommand(product.Id, newName, product.Price);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);

            var handler = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newName, result.Value.Name);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
        }

        [Fact]
        public async Task UpdateProductCommandHandler_ValidDetailsAndUpdateThePrice_ProductDto()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            var newPrice = 32m;
            var request = new UpdateProductCommand(product.Id, product.Name, newPrice);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);

            var handler = mock.Create<UpdateProductCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newPrice, result.Value.Price);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
        }
    }
}
