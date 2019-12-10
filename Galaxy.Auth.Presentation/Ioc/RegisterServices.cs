using System.Linq;
using System.Reflection;
using Galaxy.Auth.Core.Helpers;
using Galaxy.Auth.Core.Interfaces;
using Galaxy.Auth.Core.UserManagerExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace Galaxy.Auth.Presentation.Ioc
{
    public  static class RegisterServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var dataAssembly = Assembly.Load("Galaxy.Auth.Core");

            dataAssembly.GetTypesForPath("Galaxy.Auth.Core.Services")
                .ForEach(p =>
                {
                    var interfaceValue = p.GetInterfaces().FirstOrDefault();

                    if (interfaceValue != null)
                    {
                        services.AddScoped(interfaceValue.UnderlyingSystemType, p.UnderlyingSystemType);
                    }
                });
            
            // add interfaces for user manager in order to be able to unit test 
            services.AddScoped(typeof(ISignInManager<>), typeof(ExtendedSignInManager<>));
            services.AddScoped(typeof(IUserManager<>), typeof(ExtendedUserManager<>));
            services.AddScoped(typeof(IUrlEncoder), typeof(UrlEncoder));
            return services;
        }
    }
}