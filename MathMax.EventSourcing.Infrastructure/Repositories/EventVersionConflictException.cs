using System;

namespace MathMax.EventSourcing.Infrastructure.Repositories;

public class EventVersionConflictException : Exception
{
    public EventVersionConflictException()
    {
    }

    public EventVersionConflictException(string? message) : base(message)
    {
    }

    public EventVersionConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public static EventVersionConflictException ForEvent(Guid? aggregateId, int? version, string eventType)
    {
        return new EventVersionConflictException(
            $"Unable to register event of type {eventType} with AggregateId = {aggregateId} and Version = {version}. Another event with the same AggregateId and Version already exists.");
    }
}