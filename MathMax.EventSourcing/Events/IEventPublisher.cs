// A minimal but structured example of an event-sourced C# application
// using ASP.NET Core API, PostgreSQL for event storage, and best practices for
// per-aggregate versioning, optimistic concurrency, and clean command handling.

using System.Threading.Tasks;

namespace MathMax.EventSourcing;

public interface IEventPublisher
{
    Task PublishAsync(EventEnvelope envelope);
}
