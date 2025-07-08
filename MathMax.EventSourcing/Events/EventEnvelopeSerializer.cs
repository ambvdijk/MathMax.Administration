using System;
using System.Text.Json;

namespace MathMax.EventSourcing;

public class EventEnvelopeSerializer : IEventEnvelopeSerializer
{
    public EventEnvelope Serialize<TEvent>(EventEnvelope<TEvent> envelope)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(envelope);

        if (envelope.Payload == null)
        {
            throw EventSerializationException<TEvent>.ForEmptyEvent(envelope);
        }

        // Serialize to Utf8JsonWriter-compatible buffer
        byte[] serializedPayload = JsonSerializer.SerializeToUtf8Bytes(envelope.Payload);

        // Parse directly to JsonDocument
        using JsonDocument payload = JsonDocument.Parse(serializedPayload);


        if (payload == null || payload.RootElement.ValueKind == JsonValueKind.Undefined || payload.RootElement.ValueKind == JsonValueKind.Null)
        {
            throw EventSerializationException<TEvent>.ForEmptyEvent(envelope);
        }

        return new EventEnvelope(
            envelope.UniqueId,
            envelope.ReadableId,
            envelope.EventType,
            payload,
            envelope.AggregateId,
            envelope.Version,
            envelope.Timestamp
        );
    }

    public EventEnvelope<TEvent> Deserialize<TEvent>(EventEnvelope envelope)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(envelope);

        try
        {
            if (envelope.Payload == null)
            {
                throw EventDeserializationException.ForEmptyEvent(envelope);
            }

            var @event = JsonSerializer.Deserialize<TEvent>(envelope.Payload);

            if (@event == null)
            {
                throw EventDeserializationException.ForEmptyEvent(envelope);
            }

            return new EventEnvelope<TEvent>(
                envelope.UniqueId,
                envelope.ReadableId,
                envelope.EventType,
                @event,
                envelope.AggregateId,
                envelope.Version,
                envelope.Timestamp
            );
        }
        catch (Exception ex)
        {
            throw EventDeserializationException.ForEvent(ex, envelope);
        }
    }
}
