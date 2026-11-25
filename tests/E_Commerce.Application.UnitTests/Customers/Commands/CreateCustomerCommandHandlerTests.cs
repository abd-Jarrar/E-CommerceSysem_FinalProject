using Autofac.Extras.Moq;
using E_Commerce.Application.Abstractions;
using E_Commerce.Application.Customers.Commands.CreateCustomer;
using E_Commerce.Domain.Customers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace E_Commerce.Application.UnitTests.Customers.Commands
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly CancellationToken ct=CancellationToken.None;
        [Fact]
        public async Task CreateCustomerCommandHandler_UsedEmail_EmailExists()
        {
            //Arrange
            var request = new CreateCustomerCommand("", "test@gmail.com", true);
            using var mock = AutoMock.GetLoose();

            mock.Mock<ICustomerRepository>()
       .Setup(x => x.EmailExistsAsync(request.Email, ct))
       .ReturnsAsync(true);

            var handler = mock.Create<CreateCustomerCommandHandler>();
            //Act
            var result = await handler.Handle(request, ct);
            //Assert
            Assert.Contains(CustomerErrors.EmailExists, result.Errors);
        }
        [Fact]
        public async Task CreateCustomerCommandHandler_EmptyName_NameRequired()
        {
            //arrange
            var request = new CreateCustomerCommand("", "test@gmail.com", true);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>().Setup(x=>x.EmailExistsAsync(request.Email,ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateCustomerCommandHandler>();
            //act
            var result =await handler.Handle(request,ct); 
            //assert
            Assert.Contains(CustomerErrors.NameRequired, result.Errors);
        }
        [Fact]
        public async Task CreateCustomerCommandHandler_EmptyEmail_EmailRequired()
        {
            //arrange
            var request = new CreateCustomerCommand("name", "", true);
            using var mock= AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>().Setup(x => x.EmailExistsAsync(request.Email, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateCustomerCommandHandler>();
            //act
            var result=await handler.Handle(request,ct);
            //assert
            Assert.Contains(CustomerErrors.EmailRequired, result.Errors);
        }
        [Fact]
        public async Task CreateCustomerCommandHandler_InvalidEmail_EmailInvalid()
        {
            //arrange
            var request = new CreateCustomerCommand("name", "wrong format", false);
            using var mock=AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>().Setup(x => x.EmailExistsAsync(request.Email, ct)).ReturnsAsync(false);
            var handler=mock.Create<CreateCustomerCommandHandler>();
            //act
            var result= await handler.Handle(request,ct);
            //assert
            Assert.Contains(CustomerErrors.EmailInvalid, result.Errors);
        }
        [Fact]
        public async Task CreateCustomerCommandHandler_ValidData_CustomerDto()
        {
            //arrange
            var request = new CreateCustomerCommand("name", "test@gmail.com", false);
            using var mock = AutoMock.GetLoose();
            mock.Mock<ICustomerRepository>().Setup(x => x.EmailExistsAsync(request.Email, ct)).ReturnsAsync(false);
            var handler = mock.Create<CreateCustomerCommandHandler>();
            //act
            var result = await handler.Handle(request,ct);
            //assert
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            mock.Mock<ICustomerRepository>().Verify(x => x.AddAsync(It.IsAny<Customer>(), ct), Times.Once);
            mock.Mock<IUnitOfWork>().Verify(x => x.SaveChangesAsync(ct), Times.Once);
            Assert.Equal("name", result.Value.Name);
            Assert.Equal("test@gmail.com", result.Value.Email);
            Assert.False(result.Value.IsVip);

        }
    }
}
