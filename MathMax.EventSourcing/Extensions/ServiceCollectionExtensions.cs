using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.EventSourcing.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically registers all command handlers that implement ICommandHandler<TCommand, TEvent>
    /// from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies to scan for command handlers. If null, scans the calling assembly.</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        var commandHandlerInterface = typeof(ICommandHandler<,>);

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == commandHandlerInterface))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var implementedInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == commandHandlerInterface);

                foreach (var implementedInterface in implementedInterfaces)
                {
                    services.AddScoped(implementedInterface, handlerType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Automatically registers all command handlers that implement ICommandHandler<TCommand, TEvent>
    /// from the current assembly.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        return services.AddCommandHandlers(Assembly.GetCallingAssembly());
    }
}
