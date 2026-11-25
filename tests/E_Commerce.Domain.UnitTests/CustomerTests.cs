using E_Commerce.Domain.Customers;
using E_Commerce.Domain.Products;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Tests
{
    public class CustomerTests
    {
        [Fact]
        public void MarkAsVip_NormalCustomer_UpdatedSuccessfully()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result=customer.MarkAsVip();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(customer.IsVip);
        }
        [Fact]
        public void MarkAsVip_VIPCustomer_UpdatedSuccessfully()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", true).Value;
            //Act
            var result = customer.MarkAsVip();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(customer.IsVip);
        }
        [Fact]
        public void RemoveVip_NormalCustomer_UpdatedSuccessfully()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result = customer.RemoveVip();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.False(customer.IsVip);
        }
        [Fact]
        public void RemoveVip_VIPCustomer_UpdatedSuccessfully()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", true).Value;
            //Act
            var result = customer.RemoveVip();
            //Assert
            Assert.True(result.IsSuccess);
            Assert.False(customer.IsVip);
        }

        [Fact]
        public void Update_ValidName_UpdateSuccesfully()
        {
            //Arrange
            var result = Customer.Create("customer1", "test@gmail.com", false);
            var customer = result.Value;
            //Act
            customer.Update("customer2", "test@gmail.com");
            //Assert
            Assert.Equal("customer2", customer.Name);
            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void Update_EmptyName_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result = customer.Update("", "test@gmail.com");
            //Assert
            Assert.Equal("customer1", customer.Name);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.NameRequired);
        }
        [Fact]
        public void Update_WhiteSpacesName_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result=customer.Update("       ", "test@gmail.com");
            //Assert
            Assert.Equal("customer1", customer.Name);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.NameRequired);
        }
        [Fact]
        public void Update_NullName_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result=customer.Update(null!, "test@gmail.com");
            //Assert
            Assert.Equal("customer1", customer.Name);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.NameRequired);


        }
        [Fact]
        public void Update_ValidEmail_UpdateSuccesfully()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result=customer.Update("customer1", "test2@gmail.com");
            //Assert
            Assert.Equal("test2@gmail.com", customer.Email);
            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void Update_EmptyEmail_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result= customer.Update("customer1", "");
            //Assert
            Assert.Equal("test@gmail.com", customer.Email);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.EmailRequired);
        }
        [Fact]
        public void Update_WhiteSpacesEmail_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result = customer.Update("customer1", "    ");
            //Assert
            Assert.Equal("test@gmail.com", customer.Email);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.EmailRequired);
        }
        [Fact]
        public void Update_NullEmail_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result= customer.Update("customer1", null!);
            //Assert
            Assert.Equal("test@gmail.com", customer.Email);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.EmailRequired);

        }
        [Fact]
        public void Update_InvalidEmailFormat_FailToUpdate()
        {
            //Arrange
            var customer = Customer.Create("customer1", "test@gmail.com", false).Value;
            //Act
            var result=customer.Update("customer1", "test@com");
            //Assert
            Assert.Equal("test@gmail.com", customer.Email);
            Assert.True(result.IsError);
            Assert.Equal(result.TopError, CustomerErrors.EmailInvalid);

        }
        [Fact]
        public void Create_VipCustomerWithValidDetails_CreatedSuccessfully()
        {
            //Act
            var result = Customer.Create("customer1", "customer@gmail.com",true);
            var customer= result.Value;
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("customer1", customer.Name);
            Assert.Equal("customer@gmail.com", customer.Email);
            Assert.True(customer.IsVip);
        }
        [Fact]
        public void Create_NormalCustomerWithValidDetails_CreatedSuccessfully()
        {
            //Act
            var result = Customer.Create("customer1", "customer@gmail.com", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("customer1", customer.Name);
            Assert.Equal("customer@gmail.com", customer.Email);
            Assert.False(customer.IsVip);
        }
        [Fact]
        public void Create_EmptyName_FailToCreate()
        {
            //Act
            var result = Customer.Create("", "customer@gmail.com", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.NameRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_WhiteSpacesName_FailToCreate()
        {
            //Act
            var result = Customer.Create("   ", "customer@gmail.com", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.NameRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_NullName_FailToCreate()
        {
            //Act
            var result = Customer.Create(null!, "customer@gmail.com", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.NameRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_EmptyEmail_FailToCreate()
        {
            //Act
            var result = Customer.Create("customer1", "", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.EmailRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_WhiteSpacesEmail_FailToCreate()
        {
            //Act
            var result = Customer.Create("customer1", "    ", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.EmailRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_NullEmail_FailToCreate()
        {
            //Act
            var result = Customer.Create("customer1", null!, false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.EmailRequired, result.TopError);
            Assert.Null(customer);
        }
        [Fact]
        public void Create_InvalidEmailFormat_FailToCreate()
        {
            //Act
            var result = Customer.Create("customer1", "test@com", false);
            var customer = result.Value;
            //Assert
            Assert.True(result.IsError);
            Assert.Equal(CustomerErrors.EmailInvalid, result.TopError);
            Assert.Null(customer);

        }

    }
}
