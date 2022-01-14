using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Authors;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class WebApiController : Controller
    {
        protected readonly IHostEnvironment Environment;
        protected Guid UserId;

        protected Author Author;
        protected AppUser AppUser;


        public WebApiController(IHostEnvironment environment)
        {
            Environment = environment;
            UserId = default;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            CheckModel(context);
            GetUserIdAsync(context);
            return base.OnActionExecutionAsync(context, next);
        }


        private void GetUserIdAsync(ActionExecutingContext context)
        {
            var userId = User
                .Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                .Value;
            Guid.TryParse(userId, out UserId);
            if (UserId == default) return;

            context.HttpContext.Items["UserId"] = userId;
            context.HttpContext.Items["UserEmail"] = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (User.IsInRole("Author"))
            {
                var authorService = HttpContext.RequestServices.GetService<IAuthorService>();
                Author = authorService.GetByUserId(UserId);
                if (Author == null)
                    context.Result = Unauthorized();
            }
        }

        private void CheckModel(ActionExecutingContext context)
        {
            if (!ModelState.IsValid)
            {
                context.Result = BadRequest(
                    ModelState
                        .Values
                        .SelectMany(x => x.Errors
                            .Select(y => y.ErrorMessage))
                        .ToList()
                );
            }
        }


        [NonAction]
        public void SetUserId(Guid userId)
        {
            UserId = userId;
        }

        [NonAction]
        public string GetBearerToken(string authorization)
        {
            return AuthenticationHeaderValue.TryParse(authorization, out var headerValue) ? headerValue.Parameter : null;
        }
        
        [NonAction]
        public async Task<Author> GetAuthorByTokenAsync(string authorization)
        {
            var token = GetBearerToken(authorization);

            if (string.IsNullOrEmpty(token)) return null;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "nameid");

            if (nameClaim != null && Guid.TryParse(nameClaim.Value, out Guid id))
            {
                var authorService = HttpContext.RequestServices.GetService<IAuthorService>();
                return await authorService.GetAuthorByIdAsync(id);
            }

            return null;
        }
    }
}
