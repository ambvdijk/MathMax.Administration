using System;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Services;

namespace MathMax.EventSourcing.UnitTests.Commands;

// Test command and event classes
public class TestCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestEvent
{
    public Guid CommandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

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
