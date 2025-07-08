using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NATS.Client.Core;
using Microsoft.Extensions.Configuration;
using MathMax.EventSourcing.Extensions;

namespace MathMax.EventSourcing;

public class NatsEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly NatsConnection _connection;

    public NatsEventPublisher(IConfiguration configuration)
    {
        var natsUrl = configuration.GetRequiredValue("NATS:Url");
        var opts = new NatsOpts { Url = natsUrl };
        _connection = new NatsConnection(opts);
    }

    public async Task PublishAsync(EventEnvelope envelope)
    {
        var json = JsonSerializer.Serialize(envelope);
        
        // Compose subject as "events.{eventType}", e.g. "events.CustomerCreated"
        var subject = $"events.{envelope.EventType}";

        await _connection.PublishAsync(subject, Encoding.UTF8.GetBytes(json));
    }

    public async ValueTask DisposeAsync()
    {
        // Call async cleanup
        await _connection.DisposeAsync();
    }

}
