using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MathMax.EventSourcing.Infrastructure.Repositories;


namespace MathMax.EventSourcing;

public class PostgresEventStore : IEventStore
{
    private readonly IEventRepository _eventRepository;

    public PostgresEventStore(IEventRepository eventRepository)
    {
        ArgumentNullException.ThrowIfNull(eventRepository);
        _eventRepository = eventRepository;
    }

    // Load all events for an aggregate ordered by version
    public Task<IReadOnlyList<EventEnvelope>> LoadEventsAsync(Guid aggregateId)
    {
        throw new NotImplementedException("LoadEventsAsync is not implemented yet.");
    }

    // Append event only if expected version matches to prevent concurrency conflicts
    public async Task AppendEventAsync(EventEnvelope envelope)
    {
        if(!await _eventRepository.AppendEventAsync(envelope))
        {
            throw EventVersionConflictException.ForEvent(envelope.AggregateId, envelope.Version, envelope.EventType);
        }
    }
}
