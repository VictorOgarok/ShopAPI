using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShopAPI.Options;
using System.Text;
using Microsoft.OpenApi.Models;
using ShopAPI.Services;

namespace ShopAPI.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            var jwtOptions = new JwtOptions();
            configuration.Bind(nameof(jwtOptions), jwtOptions);
            services.AddSingleton(jwtOptions);

            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(i =>
            {
                i.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                i.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                i.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(i =>
            {
                i.SaveToken = true;
                i.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddScoped<IAuthService, AuthService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Shop API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insert your security description here",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
        }
    }
}
