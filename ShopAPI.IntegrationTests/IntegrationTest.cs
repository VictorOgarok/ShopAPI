using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShopAPI.Data;
using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Http.Json;
using ShopAPI.Contracts.V1;
using ShopAPI.Contracts.V1.Requests;
using ShopAPI.Contracts.V1.Responses;
using System.Linq;

namespace ShopAPI.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient client;
        private readonly IServiceProvider serviceProvider;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));

                        services.Remove(descriptor);
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDB");
                        });
                    });
                });
            serviceProvider = appFactory.Services;
            client = appFactory.CreateClient();


        }

        protected async Task Authenticate()
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<ProductResponse> AddProductAsync(CreateProductRequest request)
        {
            var response = await client.PostAsJsonAsync(ApiRoutes.Products.Create, request);
            return await response.Content.ReadAsAsync<ProductResponse>();
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await client.PostAsJsonAsync(ApiRoutes.Auth.Register, new UserRegistrationRequest()
            {
                Email = "pizzaEater@test.com",
                Password = "P1zz@!"
            });
            var registrationResponse = await response.Content.ReadAsAsync<SuccessAuthResponse>();
            var registrationResponseAlt = await response.Content.ReadAsAsync<FailedAuthResponse>();
            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var serviceScope = serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}
