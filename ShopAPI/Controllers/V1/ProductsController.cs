using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ShopAPI.Contracts.V1;
using ShopAPI.Domain;
using ShopAPI.Contracts.V1.Requests;
using ShopAPI.Contracts.V1.Responses;
using System.Linq;
using ShopAPI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShopAPI.Extensions;

namespace ShopAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        public ProductsController(IProductService service)
        {
            productService = service;
        }

        [HttpGet(ApiRoutes.Products.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await productService.GetProductsAsync());
        }

        [HttpGet(ApiRoutes.Products.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id) 
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut(ApiRoutes.Products.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest productRequest)
        {
            var userCreatedProduct = await productService.UserCreatedProductAsync(id, HttpContext.GetUserByid());

            if (!userCreatedProduct)
            {
                return BadRequest(new { error = "You didn't create this product" });
            }

            var product = await productService.GetProductByIdAsync(id);

            product.Name = productRequest.Name;

            var success = await productService.UpdateProductAsync(product);

            if (success)
            {
                return Ok(product);

            }
            return NotFound();
        }

        [HttpPost(ApiRoutes.Products.Create)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest productRequest)
        {
            var product = new Product()
            {
                Name = productRequest.Name,
                CreatorId = HttpContext.GetUserByid()
            };

            await productService.CreateProductAsync(product);

            var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/{ApiRoutes.Products.Get.Replace("{id}", product.Id.ToString())}";

            var productResponse = new ProductResponse() { Id = product.Id, Name=product.Name };

            return Created(url, productResponse);
        }

        [HttpDelete(ApiRoutes.Products.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var success = await productService.DeleteProductAsync(id);

            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
