using System;
using Galaxy.Auth.Data;
using Galaxy.Auth.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("UsersDb")));

            services.AddDefaultIdentity<User>(config =>
            {
                config.Lockout.AllowedForNewUsers = true;
                config.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 5, 0);
                config.Lockout.MaxFailedAccessAttempts = 5;
                config.Password.RequireDigit = true;
                config.Password.RequiredLength = 7;
                config.Password.RequiredUniqueChars = 1;
                config.Password.RequireLowercase = true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireUppercase = true;
                config.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            services.AddControllers();
            var build = services.BuildServiceProvider();
            
            // need to do data migrations before we set DataProtection
            var scope = build.GetService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<AuthDbContext>().Database.Migrate();


            // update the data protection options in order to persist them in database
            services.AddDataProtection()
                .SetApplicationName("Galaxy.Auth")
                .AddKeyManagementOptions(options => options.XmlRepository = build.GetService<IXmlRepository>())
                // instead of this we could use a certificate if we add it to the solution
                // this is used to keep the key encrypted in the database
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
