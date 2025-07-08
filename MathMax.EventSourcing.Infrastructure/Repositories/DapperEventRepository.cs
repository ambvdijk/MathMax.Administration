using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

using MathMax.EventSourcing;
using MathMax.EventSourcing.Extensions;

namespace MathMax.EventSourcing.Infrastructure.Repositories;

/// <summary>
/// Dapper-based implementation of IEventRepository for PostgreSQL.
/// This implementation provides better performance compared to Entity Framework
/// for simple CRUD operations on events.
/// </summary>
public class DapperEventRepository : IEventRepository
{
    private readonly string _connectionString;

    public DapperEventRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetRequiredValue("ConnectionStrings:Postgres");
    }

    public async Task<bool> AppendEventAsync(EventEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        const string sql = @"
            INSERT INTO events (unique_id, readable_id, event_type, payload, aggregate_id, version, timestamp)
            VALUES (@UniqueId, @ReadableId, @EventType, @Payload, @AggregateId, @Version, @Timestamp)
            ON CONFLICT (aggregate_id, version) DO NOTHING;";

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Use NpgsqlCommand directly for better parameter control
        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@UniqueId", envelope.UniqueId));
        command.Parameters.Add(new NpgsqlParameter("@ReadableId", envelope.ReadableId));
        command.Parameters.Add(new NpgsqlParameter("@EventType", envelope.EventType));
        command.Parameters.Add(new NpgsqlParameter("@Payload", NpgsqlDbType.Jsonb) { Value = envelope.Payload });
        command.Parameters.Add(new NpgsqlParameter("@AggregateId", envelope.AggregateId));
        command.Parameters.Add(new NpgsqlParameter("@Version", envelope.Version));
        command.Parameters.Add(new NpgsqlParameter("@Timestamp", envelope.Timestamp));

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected == 1;
    }

}
