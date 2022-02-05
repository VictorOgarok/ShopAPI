using Microsoft.AspNetCore.Mvc;
using ShopAPI.Contracts.V1;
using ShopAPI.Contracts.V1.Requests;
using ShopAPI.Contracts.V1.Responses;
using ShopAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Controllers.V1
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService service)
        {
            authService = service;
        }

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedAuthResponse
                {
                    ErrorMessages = ModelState.Values.SelectMany(i => i.Errors.Select(j => j.ErrorMessage))
                });
            }

            var response = await authService.RegisterAsync(request.Email, request.Password);

            if (!response.Success)
            {
                return BadRequest(new FailedAuthResponse
                {
                    ErrorMessages = response.ErrorMessages
                });
            }

            return Ok(new SuccessAuthResponse
            {
                Token = response.Token
            });
        }

        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var response = await authService.LoginAsync(request.Email, request.Password);

            if (!response.Success)
            {
                return BadRequest(new FailedAuthResponse
                {
                    ErrorMessages = response.ErrorMessages
                });
            }

            return Ok(new SuccessAuthResponse
            {
                Token = response.Token
            });
        }
    }
}
