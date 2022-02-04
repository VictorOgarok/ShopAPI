using Microsoft.EntityFrameworkCore;
using ShopAPI.Data;
using ShopAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext dataContext;

        public ProductService(DataContext context)
        {
            dataContext = context;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await GetProductByIdAsync(id);
            if (product == null) return false;
            dataContext.Products.Remove(product);
            var count = await dataContext.SaveChangesAsync();

            return count > 0;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await dataContext.Products.SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await dataContext.Products.ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            dataContext.Products.Update(product);
            var count = await dataContext.SaveChangesAsync();
            return count > 0;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            await dataContext.AddAsync(product);
            var count = await dataContext.SaveChangesAsync();
            return count > 0;
        }
    }
}
