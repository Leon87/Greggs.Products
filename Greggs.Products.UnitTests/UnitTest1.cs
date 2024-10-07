using Greggs.Products.Api.Configuration;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class UnitTest1
{
    private readonly Mock<IDataAccess<Product>> DataAccessMock = new();
    private readonly Mock<ILogger<ProductService>> Loggermock = new();
    private IProductService ProductService;
    private ProductController ProductController;
    private Settings Settings = new();


    [Fact]
    public async void WhenGetEndpoint_DataReturnedFromDataAccess()
    {
        ProductService = new ProductService(DataAccessMock.Object, Settings, Loggermock.Object);
        ProductController = new ProductController(ProductService);
        await ProductController.Get();
        DataAccessMock.Verify(d => d.List(It.IsAny<int?>(), It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(1.5, 4, 6)]
    [InlineData(1.5, 10, 15)]
    [InlineData(3, 4, 12)]
    [InlineData(0.8, 1, 0.8)]
    public async void WhenExchangeRateIsSet_AndCurrencyCodeIsListed_CorrectValueIsReturned(decimal currencyExchangeRate, decimal productPricePounds, decimal expectedCalculation)
    {
        Settings.CurrencyValues.Add("EU", currencyExchangeRate);
        DataAccessMock.Setup(x => x.List(It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(new List<Product> {
                new Product() {
                    Name = "Fake Product",
                    PriceInPounds = productPricePounds }
            });

        ProductService = new ProductService(DataAccessMock.Object, Settings, Loggermock.Object);
        var result = await ProductService.GetProducts(1, 0, "EU");

        Assert.NotNull(result);
        Assert.True(result.First().PriceInPounds == expectedCalculation);
        DataAccessMock.Verify(d => d.List(It.IsAny<int?>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async void WhenExchangeRateIsSetIncorrectly_AndCurrencyCodeIsListedReturnsEmptyResultSet()
    {
        Settings.CurrencyValues.Add("EU", -1);
        DataAccessMock.Setup(x => x.List(It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(new List<Product> {
                new Product() {
                    Name = "Fake Product",
                    PriceInPounds = 1 }
            });

        ProductService = new ProductService(DataAccessMock.Object, Settings, Loggermock.Object);
        var result = await ProductService.GetProducts(1, 0, "EU");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async void WhenCurrencyCodeIsNotListed_ReturnsEmptyResultSet()
    {
        ProductService = new ProductService(DataAccessMock.Object, Settings, Loggermock.Object);
        ProductController = new ProductController(ProductService);
        var res = await ProductController.Get(countryCode: "EU");
        Assert.NotNull(res);
        Assert.Empty(res);
    }
}