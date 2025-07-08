
// A minimal but structured example of an event-sourced C# application
// using ASP.NET Core API, PostgreSQL for event storage, and best practices for
// per-aggregate versioning, optimistic concurrency, and clean command handling.

using System.Threading.Channels;
using System.Threading.Tasks;


namespace MathMax.EventSourcing;

public class InMemoryEventBus : IEventPublisher
{
    private readonly Channel<EventEnvelope> _channel;

    public ChannelReader<EventEnvelope> Reader => _channel.Reader;

    public InMemoryEventBus(int capacity = 100)
    {
        _channel = Channel.CreateBounded<EventEnvelope>(new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public Task PublishAsync(EventEnvelope envelope)
    {
        return _channel.Writer.WriteAsync(envelope).AsTask();
    }
}
