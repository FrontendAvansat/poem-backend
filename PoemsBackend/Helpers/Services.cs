
using Microsoft.Extensions.DependencyInjection;
using Services.Authors;
using DataLayer;
using DataLayer.Repositories;

namespace WebApi.Helpers
{
    public static class Services
    {

        public static void AddServices(this IServiceCollection services)
        { 
            services.AddScoped<IAuthorService, AuthorService>();
        }



        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

        }
    }
}
