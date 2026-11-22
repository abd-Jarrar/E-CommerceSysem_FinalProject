using E_Commerce.Domain.Common.Results;
using E_Commerce.Domain.Orders;
using E_Commerce.Domain.Products;
using System.Security.Cryptography;

namespace E_Commerce.Tests;

public class ProductTests
{
    [Fact]
    public void Deactivate_ActiveProduct_DeActivateSuccessfully()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        product.Deactivate();
        //Assert
        Assert.False(product.IsActive);
    }
    [Fact]
    public void Deactivate_InActiveProduct_StillSucceeds()
    {
        // Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        product.Deactivate();

        // Act
        var result = product.Deactivate();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(product.IsActive);
    }
    [Fact]
    public void Activate_InActiveProduct_ActivateSuccessfully()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        product.Deactivate();
        //Act
        product.Activate();
        //Assert
        Assert.True(product.IsActive);
    }
    [Fact]
    public void Activate_ActiveProduct_StillSucceeds()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        product.Activate();
        //Assert
        Assert.True(product.IsActive);
    }
    [Fact]
    public void Update_ValidName_UpdateSuccesfully()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        product.Update("product2",1.2m);
        //Assert
        Assert.Equal("product2", product.Name);
    }
    [Fact]
    public void Update_EmptyName_FailToUpdate()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result=product.Update("", 1.2m);
        //Assert
        Assert.Equal("product1", product.Name);
        Assert.True(result.IsError);
        Assert.Equal(result.TopError, ProductErrors.NameRequired);
    }
    [Fact]
    public void Update_WhiteSpacesName_FailToUpdate()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result = product.Update("    ", 1.2m);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal("product1", product.Name);
        Assert.Equal(result.TopError, ProductErrors.NameRequired);
    }
    [Fact]
    public void Update_NullName_FailToUpdate()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result = product.Update(null!, 1.2m);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal("product1", product.Name);
        Assert.Equal(result.TopError, ProductErrors.NameRequired);


    }
    [Fact]
    public void Update_PositivePrice_UpdateSuccesfully()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result = product.Update("product1", 2.2m);
        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2.2m, product.Price);

    }
    [Fact]
    public void Update_NegativePrice_FailToUpdate()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result = product.Update("product1", -2.2m);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(1.2m, product.Price);
        Assert.Equal(result.TopError, ProductErrors.NegativeOrZeroPrice);

    }
    [Fact]
    public void Update_ZeroPrice_FailToUpdate()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        //Act
        var result = product.Update("product1", 0);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(1.2m, product.Price);
        Assert.Equal(result.TopError, ProductErrors.NegativeOrZeroPrice);
    }
    [Fact]
    public void IncreaseStock_PositiveNumber_IncreaseSuccessfuly()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.IncreaseStock(2);
        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(oldStock+2, product.AvailableStock);

    }
    [Fact]
    public void IncreaseStock_NegativeNumber_FailToIncrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.IncreaseStock(-2);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InvalidQuantity, result.TopError);
    }
    [Fact]
    public void IncreaseStock_InActiveProduct_FailToIncrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        product.Deactivate();
        //Act
        var result = product.IncreaseStock(2);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock , product.AvailableStock);
        Assert.Equal(ProductErrors.InActive, result.TopError);


    }
    [Fact]
    public void IncreaseStock_ZeroNumber_FailToIncrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.IncreaseStock(0);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InvalidQuantity, result.TopError);
    }
    [Fact]
    public void DecreaseStock_PositiveNumberAndEnoughStock_DecreaseSuccessfuly()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.DecreaseStock(5);
        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(oldStock - 5, product.AvailableStock);
    }
    [Fact]
    public void DecreaseStock_PositiveNumberAndNotEnoughStock_FailToDecrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.DecreaseStock(11);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InsufficientStock(11,10), result.TopError);
    }
    [Fact]
    public void DecreaseStock_NegativeNumber_FailToDecrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.DecreaseStock(-11);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InvalidQuantity, result.TopError);
    }
    [Fact]
    public void DecreaseStock_ZeroNumber_FailToDecrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        //Act
        var result = product.DecreaseStock(0);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InvalidQuantity, result.TopError);

    }
    [Fact]
    public void DecreaseStock_InActiveProduct_FailToDecrease()
    {
        //Arrange
        var product = Product.Create("sku", "product1", 1.2m, 10).Value;
        var oldStock = product.AvailableStock;
        product.Deactivate();
        //Act
        var result = product.DecreaseStock(5);
        //Assert
        Assert.True(result.IsError);
        Assert.Equal(oldStock, product.AvailableStock);
        Assert.Equal(ProductErrors.InActive, result.TopError);
    }
    [Fact]
    public void Create_ValidDetails_CreatedSuccessfully()
    {
        //Act
        var result = Product.Create("sku", "product1", 1.2m, 10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("sku", product.Sku);
        Assert.Equal("product1", product.Name);
        Assert.Equal(1.2m, product.Price);
        Assert.Equal(10, product.AvailableStock);
    }
    [Fact]
    public void Create_InvalidName_FailToCreate()
    {
        //Act
        var result = Product.Create("sku", "", 1.2m, 10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsError);
        Assert.Null(product);
        Assert.Equal(ProductErrors.NameRequired, result.TopError);
    }
    [Fact]
    public void Create_InvalidSKU_FailToCreate()
    {
        //Act
        var result = Product.Create("", "product1", 1.2m, 10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsError);
        Assert.Null(product);
        Assert.Equal(ProductErrors.SkuRequired, result.TopError);
    }
    [Fact]
    public void Create_NegativeStock_FailToCreate()
    {
        //Act
        var result = Product.Create("sku", "product1", 1.2m, -10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsError);
        Assert.Null(product);
        Assert.Equal(ProductErrors.NegativeStock, result.TopError);
    }
    [Fact]
    public void Create_NegativePrice_FailToCreate()
    {
        //Act
        var result = Product.Create("sku", "product1", -1.2m, 10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsError);
        Assert.Null(product);
        Assert.Equal(ProductErrors.NegativeOrZeroPrice, result.TopError);
    }
    [Fact]
    public void Create_ZeroPrice_FailToCreate()
    {
        //Act
        var result = Product.Create("sku", "product1",0 , 10);
        var product = result.Value;
        //Assert
        Assert.True(result.IsError);
        Assert.Null(product);
        Assert.Equal(ProductErrors.NegativeOrZeroPrice, result.TopError);
    }
}
