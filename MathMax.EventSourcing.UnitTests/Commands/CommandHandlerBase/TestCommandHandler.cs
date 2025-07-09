using System;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing.UnitTests.Commands;

// Test implementation of CommandHandlerBase
public class TestCommandHandler : CommandHandlerBase<TestCommand, TestEvent>
{
    public TestCommandHandler(IEventStore eventStore, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService)
        : base(eventStore, serializer, dateTimeService)
    {
    }

    protected override TestEvent CreateEvent(TestCommand command, DateTime timestamp)
    {
        return new TestEvent
        {
            CommandId = command.Id,
            Name = command.Name,
            ProcessedAt = timestamp
        };
    }

    protected override Guid? GetAggregateId(TestCommand command)
    {
        return command.Id;
    }

    protected override int? GetVersion(TestCommand command)
    {
        return 1;
    }

    // Public wrapper for testing the protected method
    public string PublicGetEventType()
    {
        return GetEventType();
    }
}
