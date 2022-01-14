using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using File = System.IO.File;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using WebApi.Helpers;
using DataLayer;
using DataLayer.Entities;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
    }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            AddContextAndIdentity(services);
            AddDependencies(services);
            services.AddHttpClient();
            
            services.AddControllers();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecurityKey"]))
                };
            });

            services.AddAuthorization();

            var assemblyDate = GetAssemblyDate();
            var buildNo = assemblyDate.DayOfYear + "." + (assemblyDate.Hour * 60 + assemblyDate.Minute);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Poems", Version = $"1.0 build {buildNo} on date " + assemblyDate.ToString("dd/MM hh:mm") });
                options.OperationFilter<SwaggerOperationFilter>();
                options.DocumentFilter<SwaggerDocumentFilter>();
            });

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        context.Items["Exception"] = contextFeature.Error.Message;
                        context.Items["StackTrace"] = contextFeature.Error.StackTrace;
                        switch (contextFeature.Error)
                        {
                            case BadRequestException _:
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                break;
                            case UnauthorizedException _:
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                break;
                            default:
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                break;
                        }

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new List<string>
                            {contextFeature.Error.Message}));
                    }
                });
            });
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Poems");
                c.RoutePrefix = "api/swagger";
            });

            app.UseCors("AllowAll");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDependencies(IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
        }
        private void AddContextAndIdentity(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Poems")));
            services
                .AddIdentity<AppUser, IdentityRole<Guid>>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequiredLength = 8;
            }
            );
        }

        private static DateTime GetAssemblyDate()
        {
            string fileName = Assembly.GetExecutingAssembly().Location;
            return File.GetLastWriteTimeUtc(fileName);
        }
    }
}
