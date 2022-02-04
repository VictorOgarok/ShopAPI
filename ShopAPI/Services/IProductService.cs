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
        List<Product> GetProducts();

        Product GetProductById(Guid id);
    }
}
