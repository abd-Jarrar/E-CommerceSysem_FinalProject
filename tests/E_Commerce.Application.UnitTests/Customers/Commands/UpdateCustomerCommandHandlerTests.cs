using Autofac.Extras.Moq;
using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Caching;
using E_Commerce.Application.Customers.Commands.UpdateCustomer;
using E_Commerce.Domain.Customers;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.UnitTests.Customers.Commands
{
    public class UpdateCustomerCommandHandlerTests
    {
        private readonly CancellationToken ct = CancellationToken.None;
        [Fact]
        public async Task UpdateCustomerCommandHandler_CustomerNotFound_NotFound()
        {
            //arrange
            Guid guid= Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "name", "test@email.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>().Setup(x => x.GetByIdAsync(request.Id, ct)).ReturnsAsync(default(Customer));
            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.NotFound(request.Id), result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateEmailToUsedEmail_EmailExists()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "name", "test@email.com", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x=>x.EmailExistsAsync(request.Email,ct))
                .ReturnsAsync(true);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.EmailExists, result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateNameToEmptyName_NameRequired()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "", "test@email.com", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.NameRequired, result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateNameToNullName_NameRequired()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, null!, "test@email.com", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.NameRequired, result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateEmailToEmtpyEmail_EmailRequired()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "Name", "", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.EmailRequired, result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateEmailToInvalidEmail_EmailRequired()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "Name", "tests", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.Contains(CustomerErrors.EmailInvalid, result.Errors);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateEmailToValidEmail_CustomerDto()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "Name", "test2@gmail.com", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Email, result.Value.Email);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
            mock.Mock<HybridCache>().Verify(x => x.RemoveAsync(CustomerCacheKeys.ToCacheKey(customer.Id), ct), Times.Once);

        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateNameToValidName_CustomerDto()
        {
            //arrange
            bool isVip = true;
            var customer = Customer.Create("Name", "test1@email.com", isVip).Value;
            var request = new UpdateCustomerCommand(customer.Id, "Name2", "test1@gmail.com", isVip);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(customer);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Name, result.Value.Name);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
            mock.Mock<HybridCache>().Verify(x => x.RemoveAsync(CustomerCacheKeys.ToCacheKey(customer.Id), ct), Times.Once);
        }

    }
}
