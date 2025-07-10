using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathMax.Administration.Projections.Customer;

internal static class Program
{
    private static void Main(string[] args)
    {

        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<ProjectionSettings>(new ProjectionSettings
                {
                    NatsUrl = "nats://localhost:4222",
                    StreamName = "EVENTS",
                    ConsumerName = "projection-consumer",
                    Subject = "events.>",
                    PostgresConnectionString = "Host=localhost;Username=postgres;Password=yourpassword;Database=yourdb"
                });

                // Channel shared between reader and processor
                services.AddSingleton(Channel.CreateBounded<EventEnvelope>(100));

                services.AddHostedService<CustomerProjectionService>();
                services.AddHostedService<CustomerEventProcessor>();
            })
            .Build()
            .Run();

    }
}
