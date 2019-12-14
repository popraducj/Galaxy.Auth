using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Galaxy.Auth.Presentation.Ioc
{
    public static class RegisterRepositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var dataAssembly = Assembly.Load("Galaxy.Auth.Infrastructure");

            dataAssembly.GetTypesForPath("Galaxy.Auth.Infrastructure.Repositories")
                .ForEach(p =>
                {
                    var interfaceValue = p.GetInterfaces().FirstOrDefault();

                    if (interfaceValue != null)
                    {
                        services.AddScoped(interfaceValue.UnderlyingSystemType, p.UnderlyingSystemType);
                    }
                });
            return services;
        }
    }
}