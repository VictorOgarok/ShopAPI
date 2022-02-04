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

        public bool DeleteProduct(Guid id)
        {
            var product = GetProductById(id);

            if (product != null)
                return false;

            products.Remove(product);
            return true;
        }

        public Product GetProductById(Guid id)
        {
            return products.SingleOrDefault(i => i.Id == id);
        }

        public List<Product> GetProducts()
        {
            return products;
        }

        public bool UpdateProduct(Product product)
        {
            if (GetProductById(product.Id)!= null)
            {
                var index = products.FindIndex(i => i.Id == product.Id);
                products[index] = product;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
