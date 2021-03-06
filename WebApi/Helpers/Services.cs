
using Microsoft.Extensions.DependencyInjection;
using Services.Authors;
using DataLayer;
using DataLayer.Repositories;
using Services;

namespace WebApi.Helpers
{
    public static class Services
    {

        public static void AddServices(this IServiceCollection services)
        { 
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IAuthentificationService, AuthentificationService>();
            services.AddScoped<IUserAuthentificationHelper, UserAuthentificationHelper>();
            services.AddScoped<IPoemService, PoemService>();

        }



        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IPoemRepository, PoemRepository>();


        }
    }
}
