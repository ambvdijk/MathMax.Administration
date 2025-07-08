using System;
using System.Threading.Tasks;
using ByteAether.Ulid;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing;

/// <summary>
/// Abstract base class for command handlers that provides common functionality
/// for handling commands and creating event envelopes.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle</typeparam>
/// <typeparam name="TEvent">The type of event to produce</typeparam>
public abstract class CommandHandlerBase<TCommand, TEvent> : ICommandHandler<TCommand, TEvent>
    where TCommand : class
    where TEvent : class
{
    protected readonly IEventStore EventStore;
    protected readonly IEventEnvelopeSerializer Serializer;
    protected readonly IDateTimeService DateTimeService;

    protected CommandHandlerBase(IEventStore eventStore, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(eventStore);
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        EventStore = eventStore;
        Serializer = serializer;
        DateTimeService = dateTimeService;
    }

    /// <summary>
    /// Handles the specified command asynchronously by creating an event and storing it in the event store.
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created event envelope.</returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Generates a new ULID for the event
    /// 2. Creates a timestamp for when the event occurred
    /// 3. Creates the event from the command using the abstract CreateEvent method
    /// 4. Extracts the aggregate ID from the command
    /// 5. Determines the version for the new event
    /// 6. Gets the event type name
    /// 7. Creates an event envelope with all the collected information
    /// 8. Serializes and stores the event envelope in the event store
    /// 9. Returns the created event envelope
    /// </remarks>
    public async Task<EventEnvelope<TEvent>> HandleAsync(TCommand command)
    {
        var ulid = Ulid.New();
        var timestamp = DateTimeService.UtcNow();
        var @event = CreateEvent(command, timestamp);
        var aggregateId = GetAggregateId(command);
        var version = GetVersion(command);
        var eventType = GetEventType();

        var eventEnvelope = new EventEnvelope<TEvent>(
            ulid.ToByteArray(),
            ulid.ToString(),
            eventType,
            @event,
            aggregateId,
            version,
            timestamp
        );

        await EventStore.AppendEventAsync(Serializer.Serialize(eventEnvelope));

        return eventEnvelope;
    }

    /// <summary>
    /// Creates the event from the command and timestamp.
    /// </summary>
    /// <param name="command">The command to process</param>
    /// <param name="timestamp">The timestamp when the event occurred</param>
    /// <returns>The created event</returns>
    protected abstract TEvent CreateEvent(TCommand command, DateTime timestamp);

    /// <summary>
    /// Extracts the aggregate ID from the command.
    /// </summary>
    /// <param name="command">The command to process</param>
    /// <returns>The aggregate ID</returns>
    protected abstract Guid? GetAggregateId(TCommand command);

    /// <summary>
    /// Determines the version for the new event.
    /// </summary>
    /// <param name="command">The command to process</param>
    /// <returns>The version number for the new event</returns>
    protected abstract int? GetVersion(TCommand command);

    /// <summary>
    /// Gets the event type name for the event envelope.
    /// </summary>
    /// <returns>The event type name</returns>
    protected virtual string GetEventType()
    {
        return typeof(TEvent).Name;
    }
}
