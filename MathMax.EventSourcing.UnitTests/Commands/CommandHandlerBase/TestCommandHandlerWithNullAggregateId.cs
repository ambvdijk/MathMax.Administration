using System;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing.UnitTests.Commands;

// Test handlers with null values
public class TestCommandHandlerWithNullAggregateId : CommandHandlerBase<TestCommand, TestEvent>
{
    public TestCommandHandlerWithNullAggregateId(IEventStore eventStore, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService)
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
        return null;
    }

    protected override int? GetVersion(TestCommand command)
    {
        return 1;
    }
}
