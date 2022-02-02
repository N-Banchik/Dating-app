using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.Helpers;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiseExtensions
    {
        public static IServiceCollection AddApplicationsServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"))
            );

            return services;
        }
    }
}