using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using NATS.Client.JetStream;

namespace MathMax.Administration.Projections.Customer;

public class CustomerProjectionService : BackgroundService
{
    private readonly ProjectionSettings _settings;
    private readonly Channel<EventEnvelope> _channel;

    public CustomerProjectionService(ProjectionSettings settings, Channel<EventEnvelope> channel)
    {
        _settings = settings;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cf = new ConnectionFactory();
        using var c = cf.CreateConnection(_settings.NatsUrl);
        var jsm = c.CreateJetStreamManagementContext();
        var js = c.CreateJetStreamContext();

        try
        {
            jsm.AddOrUpdateConsumer(_settings.StreamName, new ConsumerConfiguration
            {
                Durable = _settings.ConsumerName,
                AckPolicy = AckPolicy.Explicit,
                FilterSubject = _settings.Subject
            });
        }
        catch (NATSJetStreamException)
        {
            // Consumer exists - ignore
        }

        var pullSub = js.PullSubscribe(_settings.Subject, _settings.ConsumerName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = pullSub.Pull(10, 1000);

            foreach (var msg in messages)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    var envelope = JsonSerializer.Deserialize<EventEnvelope>(Encoding.UTF8.GetString(msg.Data));
                    if (envelope == null)
                    {
                        Console.WriteLine("Failed to deserialize EventEnvelope");
                        msg.Nak();
                        continue;
                    }

                    if (envelope.EventType == nameof(CustomerCreated) || envelope.EventType == nameof(CustomerUpdated))
                    {
                        // Post to channel with backpressure handling
                        await _channel.Writer.WriteAsync(envelope, stoppingToken);
                        msg.Ack();
                    }
                    else
                    {
                        Console.WriteLine($"Skipping unknown event type: {envelope.EventType}");
                        msg.Ack(); // or Nak() if you want to retry later
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex}");
                    msg.Nak();
                }
            }
        }

        _channel.Writer.Complete();
    }
}
