using Autofac.Extras.Moq;
using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Products.Commands.DeleteProduct;
using E_Commerce.Application.Products.Dtos;
using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Products;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.UnitTests.Products.Commands
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly CancellationToken ct = CancellationToken.None;

        [Fact]
        public async Task DeleteProductCommandHandler_NotFoundProduct_NotFound()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m,20).Value;
            var request = new DeleteProductCommand(product.Id);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(default(Product));
            var hanlder= mock.Create<DeleteProductCommandHandler>();
            //act
            var result =await hanlder.Handle(request, ct);
            //assert
            Assert.Contains(ProductErrors.NotFound(product.Id), result.Errors);

        }

        [Fact]
        public async Task DeleteProductCommandHandler_ValidProduct_ProductDto()
        {
            //arrange
            var product = Product.Create("sku", "product", 20m, 20).Value;
            var request = new DeleteProductCommand(product.Id);
            using var mock = AutoMock.GetLoose();
            mock.Mock<IProductRepository>().Setup(x => x.GetByIdAsync(product.Id, ct)).ReturnsAsync(product);
            var hanlder = mock.Create<DeleteProductCommandHandler>();
            //act
            var result = await hanlder.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            mock.Mock<IUnitOfWork>().Verify(x=>x.SaveChangesAsync(ct), Times.Once());

        }
    }
}
