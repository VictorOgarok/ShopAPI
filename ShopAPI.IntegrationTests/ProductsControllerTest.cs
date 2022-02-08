using FluentAssertions;
using ShopAPI.Contracts.V1;
using ShopAPI.Contracts.V1.Requests;
using ShopAPI.Contracts.V1.Responses;
using ShopAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ShopAPI.IntegrationTests
{
    public class ProductsControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyProducts()
        {
            // Arrange
            await Authenticate();

            // Act
            var response = await client.GetAsync(ApiRoutes.Products.GetAll);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Product>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ExistingProduct()
        {
            // Arrange
            await Authenticate();
            var productResponse = await AddProductAsync(new Contracts.V1.Requests.CreateProductRequest
            {
                Name = "Pizza"
            });

            // Act
            var response = await client.GetAsync(ApiRoutes.Products.Get.Replace("{id}", productResponse.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<ProductResponse>();
            result.Id.Should().Be(productResponse.Id);
            result.Name.Should().Be("Pizza");
        }

        [Fact]
        public async Task Get_DeletedProduct()
        {
            // Arrange
            await Authenticate();
            var productResponse = await AddProductAsync(new Contracts.V1.Requests.CreateProductRequest
            {
                Name = "Temp product"
            });
            await client.DeleteAsync(ApiRoutes.Products.Delete.Replace("{id}", productResponse.Id.ToString()));

            // Act
            var response = await client.GetAsync(ApiRoutes.Products.Get.Replace("{id}", productResponse.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ExistingProduct()
        {
            // Arrange
            await Authenticate();
            var productResponse = await AddProductAsync(new Contracts.V1.Requests.CreateProductRequest
            {
                Name = "Temp product"
            });

            // Act
            var response = await client.DeleteAsync(ApiRoutes.Products.Delete.Replace("{id}", productResponse.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_ExistingProduct()
        {
            // Arrange
            await Authenticate();
            var productResponse = await AddProductAsync(new Contracts.V1.Requests.CreateProductRequest
            {
                Name = "Pizza"
            });
            var productRequest = new UpdateProductRequest
            {
                Name = "Not Pizza"
            };

            // Act
            var response = await client.PutAsJsonAsync(ApiRoutes.Products.Update.Replace("{id}", productResponse.Id.ToString()),productRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<Product>();
            result.Id.Should().Be(productResponse.Id);
            result.Name.Should().Be("Not Pizza");
        }
    }
}
