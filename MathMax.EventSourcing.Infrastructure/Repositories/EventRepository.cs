using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Npgsql;

using MathMax.EventSourcing;
using MathMax.EventSourcing.Entity;
using MathMax.EventSourcing.Infrastructure.EntityFramework;

namespace MathMax.EventSourcing.Infrastructure.Repositories;

public class EntityFrameworkEventRepository(EventContext context) : EntityFrameworkRepository<EventEnvelope, Guid>(context), IEventRepository
{
    public async Task<bool> AppendEventAsync(EventEnvelope envelope)
    {
        var sql = @"
            INSERT INTO events (unique_id, readable_id, event_type, payload, aggregate_id, version, timestamp)
            VALUES (@UniqueId, @ReadableId, @EventType, @Payload, @AggregateId, @Version, @Timestamp)
            ON CONFLICT (aggregate_id, version) DO NOTHING;";

        var parameters = new[]
        {
            new NpgsqlParameter("UniqueId", envelope.UniqueId),
            new NpgsqlParameter("ReadableId", envelope.ReadableId),
            new NpgsqlParameter("EventType", envelope.EventType),
            new NpgsqlParameter("Payload", envelope.Payload),
            new NpgsqlParameter("AggregateId", envelope.AggregateId),
            new NpgsqlParameter("Version", envelope.Version),
            new NpgsqlParameter("Timestamp", envelope.Timestamp)
        };

        return await Context.Database.ExecuteSqlRawAsync(sql, parameters) == 1;
    }

    public override Task InsertAsync(EventEnvelope entity)
    {
        throw new NotSupportedException("Inserting is not supported for events. Use AppendEventAsync instead.");
    }

    public override Task UpdateAsync(EventEnvelope entity)
    {
        throw new NotSupportedException("Updating is not supported for events. Append an changed event using AppendEventAsync instead.");
    }

    public override Task DeleteAsync(EventEnvelope entity)
    {
        throw new NotSupportedException("Deleting is not supported for events. Append a deleted event using AppendEventAsync instead.");
    }

    public override Task DeleteAsync(Guid id)
    {
        throw new NotSupportedException("Deleting is not supported for events. Append a deleted event using AppendEventAsync instead.");
    }

    public override Task RemoveRangeAsync(ICollection<EventEnvelope> objectsToRemove)
    {
        throw new NotSupportedException("Deleting is not supported for events. Append a deleted event using AppendEventAsync instead.");
    }
}
