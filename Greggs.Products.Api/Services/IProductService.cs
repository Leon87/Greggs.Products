using Greggs.Products.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts(int? take, int? skip, string countryCode);
    }
}