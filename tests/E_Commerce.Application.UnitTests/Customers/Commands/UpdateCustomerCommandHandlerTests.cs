using Autofac.Extras.Moq;
using E_Commerce.Application.Abstractions;
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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "name", "test@email.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x=>x.EmailExistsAsync(request.Email,ct))
                .ReturnsAsync(true);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create(request.Name,"random@gmail.com", request.IsVip).Value);

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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "", "test@email.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test@email.com", request.IsVip).Value);

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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, null!, "test@email.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test@email.com", request.IsVip).Value);

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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "Name", "", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test@email.com", request.IsVip).Value);

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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "Name", "tests", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test@email.com", request.IsVip).Value);

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
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "Name", "test1@gmail.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test@email.com", request.IsVip).Value);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Email, result.Value.Email);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
            mock.Mock<HybridCache>().Verify(x => x.RemoveByTagAsync("customer", ct), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerCommandHandler_UpdateNameToValidName_CustomerDto()
        {
            //arrange
            Guid guid = Guid.NewGuid();
            var request = new UpdateCustomerCommand(guid, "Name2", "test1@gmail.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>()
                .Setup(x => x.EmailExistsAsync(request.Email, ct))
                .ReturnsAsync(false);

            mock.Mock<ICustomerRepository>()
                .Setup(x => x.GetByIdAsync(request.Id, ct))
                .ReturnsAsync(Customer.Create("Name", "test1@email.com", request.IsVip).Value);

            var handler = mock.Create<UpdateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request, ct);
            //assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Name, result.Value.Name);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
            mock.Mock<HybridCache>().Verify(x => x.RemoveByTagAsync("customer", ct), Times.Once);
        }


    }
}
