using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopAPI.Data;
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
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly DataContext dataContext;

        public AuthService(UserManager<IdentityUser> manager, JwtOptions options, TokenValidationParameters validationParameters, DataContext context)
        {
            userManager = manager;
            jwtOptions = options;
            tokenValidationParameters = validationParameters;
            dataContext = context;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    ErrorMessages = new[] { "Incorrect username or password" }
                };
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
            {
                return new AuthenticationResult
                {
                    ErrorMessages = new[] { "Incorrect username or password" }
                };
            }

            return await GenerateAuthenticationResultAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "Invalid token" } };
            }

            var expiryDateInUnix = long.Parse(validatedToken.Claims.Single(i => i.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateInUtc = DateTime.UnixEpoch.AddSeconds(expiryDateInUnix);
            if (expiryDateInUtc < DateTime.UtcNow)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(i => i.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await dataContext.RefreshTokens.SingleOrDefaultAsync(i => i.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This refresh token doesn't exist" } };
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryTime)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This refresh token has expired" } };
            }
            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This refresh token is invalidated" } };
            }
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This refresh token has been used" } };
            }
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { ErrorMessages = new[] { "This refresh token doesn't match your JWT" } };
            }

            storedRefreshToken.Used = true;
            dataContext.RefreshTokens.Update(storedRefreshToken);
            dataContext.SaveChanges();

            var user = await userManager.FindByIdAsync(validatedToken.Claims.Single(i=>i.Type=="id").Value);
            return await GenerateAuthenticationResultAsync(user);
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

            var newUser = await userManager.CreateAsync(user, password);

            if (!newUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    ErrorMessages = newUser.Errors.Select(i => i.Description)
                };
            }

            return await GenerateAuthenticationResultAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(IdentityUser user)
        {
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
                Expires = DateTime.UtcNow.Add(jwtOptions.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationTime = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMonths(6)
            };

            await dataContext.RefreshTokens.AddAsync(refreshToken);
            await dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
