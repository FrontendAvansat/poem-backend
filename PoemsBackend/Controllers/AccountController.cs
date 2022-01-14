using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace WebApi.Controllers.Auth
{
    [Route("api/auth-service")]
    [ApiController]
    public class AccountController : WebApiController
    {
        //ivate readonly IAuthenticationService _authenticationService;

        public AccountController(IHostEnvironment hostEnvironment /*IAuthenticationService authenticationService*/) : base(hostEnvironment)
        {
           //authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("account/login")]
        public async Task<IActionResult> Login(/*[FromBody] LoginRequest loginRequest*/)
        {
            return Ok(true);
            //var login = await _authenticationService.LoginAsync(loginRequest, PotentialCustomer);
            //return Ok(login);
        }

        //[HttpPost]
        //[Route("account/validate-email")]
        //public async Task<IActionResult> ValidateEmail([FromBody] ValidationRequest validationRequest)
        //{
        //    await _authenticationService.ValidateEmailAsync(validationRequest, Language);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("account/register-producer")]
        //public async Task<IActionResult> RegisterProducer([FromBody] RegisterRequest registerRequest)
        //{
        //    await _authenticationService.RegisterProducerAsync(registerRequest, Language, HttpContext.Request);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("account/register-customer")]
        //public async Task<IActionResult> RegisterCustomer([FromBody] RegisterRequestCustomer registerRequest)
        //{
        //     _ = await _authenticationService.RegisterCustomerAsync(registerRequest, Language, HttpContext.Request);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("account/refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        //{
        //    var refreshToken = await _authenticationService.RefreshTokenAsync(tokenRequest);
        //    return Ok(refreshToken);
        //}

        //[HttpGet]
        //[Authorize]
        //[Route("account/who-am-i")]
        //public async Task<ActionResult<UserInformationDto>> WhoAmI([FromQuery] Guid? deliverySlotId)
        //{
        //    var user = await _authenticationService.GetUserInformationAsync(UserId, deliverySlotId);
        //    return user;
        //}

        //[HttpPost]
        //[Route("account/register-local-facilitator")]
        //public async Task<IActionResult> RegisterLocalFacilitator([FromBody] RegisterRequest registerRequest)
        //{
        //    await _authenticationService.RegisterLocalFacilitatorAsync(registerRequest, Language, HttpContext.Request);
        //    return Ok();
        //}
    }
}
