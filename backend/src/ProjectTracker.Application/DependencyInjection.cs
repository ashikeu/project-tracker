using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        // Convention-based handler registration (replaces MediatR's auto-registration): every
        // concrete class named "*Handler" is registered against every interface it implements
        // named "I*Handler" declared in this assembly. Keeps Application/Features free of a
        // manually maintained DI list as new feature slices are added.
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Handler", StringComparison.Ordinal));

        foreach (var implementationType in handlerTypes)
        {
            var handlerInterfaces = implementationType.GetInterfaces()
                .Where(i => i.Assembly == assembly && i.Name.StartsWith("I", StringComparison.Ordinal)
                            && i.Name.EndsWith("Handler", StringComparison.Ordinal));

            foreach (var serviceType in handlerInterfaces)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }

        return services;
    }
}
