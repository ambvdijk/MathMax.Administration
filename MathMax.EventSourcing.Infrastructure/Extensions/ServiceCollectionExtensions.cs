using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using MathMax.EventSourcing.Infrastructure.Repositories;

namespace MathMax.EventSourcing.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Dapper-based EventRepository implementation.
    /// Use this instead of the Entity Framework implementation for better performance.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDapperEventRepository(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, DapperEventRepository>();
        return services;
    }

    /// <summary>
    /// Registers the Entity Framework-based EventRepository implementation.
    /// This is the default implementation that requires EventContext to be registered.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEntityFrameworkEventRepository(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, EntityFrameworkEventRepository>();
        return services;
    }
}
