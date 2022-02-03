using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShopAPI.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Shop API", Version = "v1" });
            });
        }
    }
}
