using System;

namespace MathMax.EventSourcing;

public class EventDeserializationException : Exception
{
    public EventDeserializationException(string message) : base(message)
    {
    }

    public EventDeserializationException(Exception innerException, string message) : base(message, innerException)
    {
    }

    public static Exception ForEmptyEvent(EventEnvelope envelope)
    {
        return new EventDeserializationException($"Unable to deserialize Event {envelope.EventType} with UniqueId {envelope.UniqueId} and ReadableId {envelope.ReadableId}. It has an empty payload.");
    }

    public static Exception ForEvent(Exception exception, EventEnvelope envelope)
    {
        return new EventDeserializationException(exception, $"Failed to deserialize event {envelope.EventType} with UniqueId {envelope.UniqueId} and ReadableId {envelope.ReadableId}.");
    }
}

public class EventSerializationException<TEvent> : Exception
    where TEvent : class
{
    public EventSerializationException(string message) : base(message)
    {
    }

    public EventSerializationException(Exception innerException, string message) : base(message, innerException)
    {
    }

    public static Exception ForEmptyEvent(EventEnvelope<TEvent> envelope)
    {
        return new EventSerializationException<TEvent>($"Unable to serialize Event {envelope.EventType} with UniqueId {envelope.UniqueId} and ReadableId {envelope.ReadableId}. It has an empty payload.");
    }

    public static Exception ForEvent(Exception exception, EventEnvelope<TEvent> envelope)
    {
        return new EventSerializationException<TEvent>(exception, $"Failed to serialize event {envelope.EventType} with UniqueId {envelope.UniqueId} and ReadableId {envelope.ReadableId}.");
    }
}
