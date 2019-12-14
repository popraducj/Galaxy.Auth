using System;
using System.Linq;
using System.Threading.Tasks;
using Galaxy.Auth.Core.Enums;
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

        public static void InitializeDatabase(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var authDbContext = builder.GetService<AuthDbContext>();

            AddAdminUserAsync(builder).Wait();
            UpdateAdminPermissions(authDbContext);
        }

        #region Private methods

        private static async Task AddAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManagement = serviceProvider.GetRequiredService<UserManager<User>>();

            if (userManagement.Users.Count(p => p.Id == 1) > 0)
            {
                Console.WriteLine("The user has been already created");
                return;
            }

            const string email = "admin@galaxy.com";
            var user = new User
            {
                Email = email,
                EmailConfirmed = true,
                Name = "Galaxy Admin",
                UserName = Guid.NewGuid().ToString(),
                Id = 1,
                NormalizedEmail = email.ToUpper()
            };

            var createUserResult = await userManagement.CreateAsync(user, "someRandomSecurePassword123!");
            if (!createUserResult.Succeeded)
            {

                Console.WriteLine("Could not create user. " +
                                  $"Reason is: {string.Join(" ", createUserResult.Errors.Select(x => x.Description))} ");
                throw new ArgumentException("Could not create User");
            }
        }

        private static void UpdateAdminPermissions(AuthDbContext dbContext)
        {
            // making sure the admin have all permissions after each deploy
            var dbContextPermissions = dbContext.Permissions.Where(x => x.UserId ==1).ToList();
            
            dbContext.Permissions.RemoveRange(dbContextPermissions);
            dbContext.SaveChanges();
            var allPermissions = Enum.GetValues(typeof(UserPermission)).Cast<UserPermission>()
                .Select(p => new Permission
                {
                    UserPermission = p,
                    UserId = 1
                });

            dbContext.Permissions.AddRange(allPermissions);
            dbContext.SaveChanges();
        }

        #endregion
    }
}