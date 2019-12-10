using System;
using Galaxy.Auth.Core.Models;
using Galaxy.Auth.Core.UserManagerExtensions;
using Galaxy.Auth.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Galaxy.Auth.Presentation.Ioc
{
    public static class RegisterDatabase
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("AuthDb"), x => x.MigrationsAssembly("Galaxy.Auth.Presentation")));
            return services;
        }

        public static IServiceCollection AddAuthenticationRules(this IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                    config.Lockout.AllowedForNewUsers = true;
                    config.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 5, 0);
                    config.Lockout.MaxFailedAccessAttempts = 5;
                    config.Password.RequireDigit = true;
                    config.Password.RequiredLength = 8;
                    config.Password.RequiredUniqueChars = 1;
                    config.Password.RequireLowercase = true;
                    config.Password.RequireNonAlphanumeric = true;
                    config.Password.RequireUppercase = true;
                    config.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}