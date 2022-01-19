using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Services.Dtos;
using Services;

namespace WebApi.Controllers.Auth
{
    [Route("api/auth-service")]
    [ApiController]
    public class AccountController : WebApiController
    {
        private readonly IAuthentificationService _authentificationService;

        public AccountController(IHostEnvironment hostEnvironment, IAuthentificationService authentificationService) : base(hostEnvironment)
        {
           _authentificationService = authentificationService;
        }

        [HttpPost]
        [Route("account/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var login = await _authentificationService.LoginAsync(loginRequest);
            return Ok(login);
        }




        [HttpPost]
        [Route("account/register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto registerRequest)
        {
            _ = await _authentificationService.RegisterAuthorAsync(registerRequest);
            return Ok();
        }

        [HttpPost]
        [Route("account/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            var refreshToken = await _authentificationService.RefreshTokenAsync(tokenRequest);
            return Ok(refreshToken);
        }

        [HttpGet]
        [Authorize]
        [Route("account/who-am-i")]
        public ActionResult<UserInformationDto> WhoAmI()
        {
            var user =  _authentificationService.GetUserInformation(UserId);
            return user;
        }
    }
}
