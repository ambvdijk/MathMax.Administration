using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Npgsql;

using MathMax.EventSourcing;
using MathMax.Administration.Customer.Events;

namespace MathMax.Administration.Projections.Customer;

public class CustomerEventProcessor : BackgroundService
{
    private readonly ProjectionSettings _settings;
    private readonly Channel<EventEnvelope> _channel;

    public CustomerEventProcessor(ProjectionSettings settings, Channel<EventEnvelope> channel)
    {
        _settings = settings;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var conn = new NpgsqlConnection(_settings.PostgresConnectionString);
        await conn.OpenAsync(stoppingToken);

        // Ensure table exists
        await using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS customers (
                customer_id UUID PRIMARY KEY,
                name TEXT NOT NULL,
                last_updated TIMESTAMPTZ NOT NULL
            );", conn))
        {
            await cmd.ExecuteNonQueryAsync(stoppingToken);
        }

        await foreach (var envelope in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                switch (envelope.EventType)
                {
                    case nameof(CustomerCreated):
                        var created = JsonSerializer.Deserialize<CustomerCreated>(envelope.Payload);
                        if (created != null)
                            await UpsertCustomer(conn, created.CustomerId, created.Name, created.Timestamp, stoppingToken);
                        break;

                    case nameof(CustomerUpdated):
                        var updated = JsonSerializer.Deserialize<CustomerUpdated>(envelope.Payload);
                        if (updated != null)
                            await UpsertCustomer(conn, updated.CustomerId, updated.Name, updated.Timestamp, stoppingToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing event in DB: {ex}");
                // optionally add retry or dead letter handling here
            }
        }
    }

    private static async Task UpsertCustomer(NpgsqlConnection conn, Guid customerId, string name, DateTime timestamp, CancellationToken cancellationToken)
    {
        var cmdText = @"
            INSERT INTO customers (customer_id, name, last_updated)
            VALUES (@id, @name, @ts)
            ON CONFLICT (customer_id)
            DO UPDATE SET name = EXCLUDED.name, last_updated = EXCLUDED.last_updated
            WHERE customers.last_updated < EXCLUDED.last_updated;";

        await using var cmd = new NpgsqlCommand(cmdText, conn);
        cmd.Parameters.AddWithValue("id", customerId);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("ts", timestamp);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
