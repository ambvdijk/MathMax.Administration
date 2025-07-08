using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Scalar.AspNetCore;

using MathMax.EventSourcing.Entity;
using MathMax.EventSourcing.Infrastructure.Repositories;
using MathMax.EventSourcing.Extensions;
using MathMax.EventSourcing.Services;
using MathMax.EventSourcing;
using MathMax.Administration.Customer.Commands;
using MathMax.WebApi.Middleware;

namespace MathMax.Administration.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add configuration
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
        builder.Configuration.AddEnvironmentVariables();

        // Configure Entity Framework with PostgreSQL
        builder.Services.AddDbContext<EventContext>(options =>
        {
            var connectionString = builder.Configuration.GetRequiredValue("ConnectionStrings:Postgres");
            options.UseNpgsql(connectionString);
        });

        // Register repositories
        builder.Services.AddScoped<IEventRepository, DapperEventRepository>();

        // Register services
        builder.Services.AddSingleton<IDateTimeService, DateTimeService>();

        // Register event infrastructure
        builder.Services.AddSingleton<IEventEnvelopeSerializer, EventEnvelopeSerializer>();
        builder.Services.AddScoped<IEventStore, PostgresEventStore>();
        builder.Services.AddSingleton<IEventPublisher, NatsEventPublisher>();

        // Automatically register all command handlers
        builder.Services.AddCommandHandlers(typeof(UpdateCustomerHandler).Assembly);

        // Add controllers and API services
        builder.Services.AddControllers();

        // Add logging
        builder.Services.AddLogging();

        // Add health checks
        builder.Services.AddHealthChecks();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EventContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        // Logs unhandled exceptions and returns a generic error response with a trace ID
        app.UseExceptionHandling();
        
        // Logs error responses (status codes 400 and above) with method and path
        app.UseErrorResponseLogging();

        app.UseAuthorization();

        // Add health check endpoint
        app.MapHealthChecks("/health");

        app.MapControllers();

        app.Run();
    }
}
