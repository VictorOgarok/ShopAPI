using ShopAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();

        Task<Product> GetProductByIdAsync(Guid id);

        Task<bool> UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(Guid id);

        Task<bool> CreateProductAsync(Product product);
    }
}
