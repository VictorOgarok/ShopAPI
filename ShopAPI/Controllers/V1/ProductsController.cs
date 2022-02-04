using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ShopAPI.Contracts.V1;
using ShopAPI.Domain;
using ShopAPI.Contracts.V1.Requests;
using ShopAPI.Contracts.V1.Responses;
using System.Linq;
using ShopAPI.Services;

namespace ShopAPI.Controllers.V1
{
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        public ProductsController(IProductService service)
        {
            productService = service;
        }

        [HttpGet(ApiRoutes.Products.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(productService.GetProducts());
        }

        [HttpGet(ApiRoutes.Products.Get)]
        public IActionResult Get([FromRoute]Guid id)
        {
            var product = productService.GetProductById(id);

            if (product==null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost(ApiRoutes.Products.Create)]
        public IActionResult Create([FromBody] CreateProductRequest productRequest)
        {
            var product = new Product() { Id = productRequest.Id};

            if (product.Id != Guid.Empty)
                product.Id = Guid.NewGuid();

            productService.GetProducts().Add(product);

            var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/{ApiRoutes.Products.Get.Replace("{id}",product.Id.ToString())}";

            var productResponse = new ProductResponse() {Id=product.Id };

            return Created(url, productResponse);
        }
    }
}
