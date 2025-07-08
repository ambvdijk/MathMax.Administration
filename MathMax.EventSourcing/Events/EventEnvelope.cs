using System;
using System.Text.Json;

namespace MathMax.EventSourcing;

public record EventEnvelope(byte[] UniqueId, string ReadableId, string EventType, JsonDocument Payload, Guid? AggregateId, int? Version, DateTime Timestamp);
public record EventEnvelope<TEvent>(byte[] UniqueId, string ReadableId, string EventType, TEvent Payload, Guid? AggregateId, int? Version, DateTime Timestamp) where TEvent : class;
