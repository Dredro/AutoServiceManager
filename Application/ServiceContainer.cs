using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
    
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}