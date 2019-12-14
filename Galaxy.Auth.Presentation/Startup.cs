using System;
using System.IO;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.Models.Settings;
using Galaxy.Auth.Infrastructure;
using Galaxy.Auth.Infrastructure.Grpc.Services;
using Galaxy.Auth.Presentation.Ioc;
using Galaxy.Auth.Presentation.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Galaxy.Auth.Presentation
{
    public class Startup
    {
        private const string SwaggerUrl = "/swagger/v1/swagger.json";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDbContext(Configuration);
            services.AddAuthenticationRules();
            services.AddControllers();
            services.AddUserDataProtection();

            services.AddRepositories();
            services.AddServices();

            RegisterDatabase.InitializeDatabase(services);
            
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Galaxy.Auth",
                        Description =
                            "Galaxy Authentication Server used by to authenticate and create new users." +
                            "</br><strong>200</strong> Operation completed successfully" +
                            "</br><strong>400</strong> Bad Request returning a json with the fields affected and the errors encountered for each field <i>i.e. " +
                            "{ \"Email\": [\"The Email field is not a valid e-mail address.\"] }</i>" +
                            "</br><strong>500</strong> Server Error",
                        Version = "v1"
                    });
                    var xmlFile = $"{typeof(Startup).Assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                });
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.ConfigureExceptionHandler(logger.CreateLogger("Auth.GlobalLogger"));

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint(SwaggerUrl, "Galaxy.Auth"); });

            app.UseRouting();

            app.UseAuthorization();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PermissionsService>();
                endpoints.MapControllers();
            });
        }
    }
}
