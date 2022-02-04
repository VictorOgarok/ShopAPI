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
        private List<Product> products;

        public ProductService()
        {
            products = new List<Product>();
            for (int i = 0; i < 5; i++)
            {
                products.Add(new Product() { Id = Guid.NewGuid(), Name = "Sample product" });
            }
        }

        public Product GetProductById(Guid id)
        {
            return products.SingleOrDefault(i => i.Id == id);
        }

        public List<Product> GetProducts()
        {
            return products;
        }
    }
}
