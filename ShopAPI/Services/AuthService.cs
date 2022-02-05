using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShopAPI.Domain;
using ShopAPI.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JwtOptions jwtOptions;

        public AuthService(UserManager<IdentityUser> manager,JwtOptions options)
        {
            userManager = manager;
            jwtOptions = options;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return new AuthenticationResult
                {
                    ErrorMessages = new[] { "User with this email already exists" }
                };
            }

            user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var newUser = await userManager.CreateAsync(user,password);

            if (!newUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    ErrorMessages = newUser.Errors.Select(i => i.Description)
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim("id",user.Id),

                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
