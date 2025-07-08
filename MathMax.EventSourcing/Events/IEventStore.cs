using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathMax.EventSourcing;

public interface IEventStore
{
    Task<IReadOnlyList<EventEnvelope>> LoadEventsAsync(Guid aggregateId);
    Task AppendEventAsync(EventEnvelope envelope);
}
