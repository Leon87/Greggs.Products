using Greggs.Products.Api.Configuration;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataAccess<Product> _dataAccess;
        private readonly Settings _settings;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IDataAccess<Product> dataAccess, Settings settings, ILogger<ProductService> logger)
        {
            _dataAccess = dataAccess;
            _settings = settings;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetProducts(int? take = null, int? skip = null, string currencyCode = "GB")
        {
            var page = _dataAccess.List(skip, take);

            if (!currencyCode.Equals("GB", StringComparison.OrdinalIgnoreCase))
            {
                return TransformProductPrice(page, currencyCode);
            }

            return page;
        }

        private IEnumerable<Product> TransformProductPrice(IEnumerable<Product> page, string currencyCode)
        {
            if (!_settings.CurrencyValues.ContainsKey(currencyCode))
            {
                _logger.Log(LogLevel.Error, string.Format("Unable to find currency code {0}", currencyCode));
                yield break;
            }

            var exchangeRate = _settings.CurrencyValues[currencyCode];

            if (exchangeRate <= 0)
            {
                yield break;
            }

            foreach (var product in page)
            {
                yield return new Product
                {
                    PriceInPounds = product.PriceInPounds * exchangeRate,
                    Name = product.Name
                };
            }
        }
    }
}
