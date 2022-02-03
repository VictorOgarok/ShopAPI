using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ShopAPI.Contracts.V1;
using ShopAPI.Domain;

namespace ShopAPI.Controllers.V1
{
    public class ProductsController : Controller
    {
        private List<Product> _products;

        public ProductsController()
        {
            _products = new List<Product>();
            for (int i = 0; i < 5; i++)
            {
                _products.Add(new Product() { Id = Guid.NewGuid().ToString() });
            }

        }

        [HttpGet(ApiRoutes.Products.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpPost(ApiRoutes.Products.Create)]
        public IActionResult Create([FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Id))
                product.Id = Guid.NewGuid().ToString();

            _products.Add(product);

            var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/{ApiRoutes.Products.Get.Replace("{id}",product.Id)}";

            return Created(url, product);
        }
    }
}
