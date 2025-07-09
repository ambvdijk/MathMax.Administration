using System;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing.UnitTests.Commands;

public class TestCommandHandlerWithNullVersion : CommandHandlerBase<TestCommand, TestEvent>
{
    public TestCommandHandlerWithNullVersion(IEventStore eventStore, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService)
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
        return null;
    }
}
