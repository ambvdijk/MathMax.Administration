using System;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing.UnitTests.Commands;

// Spy implementation for testing method calls
public class SpyTestCommandHandler : CommandHandlerBase<TestCommand, TestEvent>
{
    public bool CreateEventCalled { get; private set; }
    public bool GetAggregateIdCalled { get; private set; }
    public bool GetVersionCalled { get; private set; }
    public bool GetEventTypeCalled { get; private set; }
    public TestCommand? LastCommand { get; private set; }
    public DateTime LastTimestamp { get; private set; }

    public SpyTestCommandHandler(IEventStore eventStore, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService)
        : base(eventStore, serializer, dateTimeService)
    {
    }

    protected override TestEvent CreateEvent(TestCommand command, DateTime timestamp)
    {
        CreateEventCalled = true;
        LastCommand = command;
        LastTimestamp = timestamp;
        return new TestEvent
        {
            CommandId = command.Id,
            Name = command.Name,
            ProcessedAt = timestamp
        };
    }

    protected override Guid? GetAggregateId(TestCommand command)
    {
        GetAggregateIdCalled = true;
        return command.Id;
    }

    protected override int? GetVersion(TestCommand command)
    {
        GetVersionCalled = true;
        return 1;
    }

    protected override string GetEventType()
    {
        GetEventTypeCalled = true;
        return base.GetEventType();
    }
}
