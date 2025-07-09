using System;
using System.Text.Json;
using ByteAether.Ulid;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Services;
using Moq;

namespace MathMax.EventSourcing.UnitTests.Commands;

/// <summary>
/// Test fixture for CommandHandlerBase tests providing shared setup and helper methods
/// </summary>
public class CommandHandlerBaseTestFixture
{
    public DateTime TestTimestamp { get; }

    public CommandHandlerBaseTestFixture()
    {
        TestTimestamp = new DateTime(2025, 7, 8, 12, 0, 0, DateTimeKind.Utc);
    }

    /// <summary>
    /// Creates a fresh set of mocks for each test
    /// </summary>
    public TestDependencies CreateTestDependencies()
    {
        var mockEventStore = new Mock<IEventStore>();
        var mockSerializer = new Mock<IEventEnvelopeSerializer>();
        var mockDateTimeService = new Mock<IDateTimeService>();
        
        mockDateTimeService.Setup(x => x.UtcNow()).Returns(TestTimestamp);
        
        var handler = new TestCommandHandler(
            mockEventStore.Object,
            mockSerializer.Object,
            mockDateTimeService.Object);

        return new TestDependencies(
            mockEventStore,
            mockSerializer,
            mockDateTimeService,
            handler);
    }

    /// <summary>
    /// Creates a test command with default or specified values
    /// </summary>
    public static TestCommand CreateTestCommand(Guid? id = null, string name = "Test Command")
    {
        return new TestCommand 
        { 
            Id = id ?? Guid.NewGuid(), 
            Name = name 
        };
    }

    /// <summary>
    /// Creates a test event based on a command
    /// </summary>
    public TestEvent CreateExpectedEvent(TestCommand command)
    {
        return new TestEvent 
        { 
            CommandId = command.Id, 
            Name = command.Name, 
            ProcessedAt = TestTimestamp 
        };
    }

    /// <summary>
    /// Creates a serialized event envelope for mocking
    /// </summary>
    public EventEnvelope CreateSerializedEnvelope(Guid? aggregateId = null, int? version = 1)
    {
        return new EventEnvelope(
            new byte[16], 
            "test-id", 
            "TestEvent", 
            JsonDocument.Parse("{}"), 
            aggregateId, 
            version, 
            TestTimestamp);
    }
}

/// <summary>
/// Container for test dependencies to ensure fresh mocks for each test
/// </summary>
public record TestDependencies(
    Mock<IEventStore> MockEventStore,
    Mock<IEventEnvelopeSerializer> MockSerializer,
    Mock<IDateTimeService> MockDateTimeService,
    TestCommandHandler Handler)
{
    /// <summary>
    /// Sets up the serializer mock to return a specific envelope
    /// </summary>
    public void SetupSerializerMock(EventEnvelope envelope)
    {
        MockSerializer.Setup(x => x.Serialize(It.IsAny<EventEnvelope<TestEvent>>()))
            .Returns(envelope);
    }

    /// <summary>
    /// Creates a spy handler for testing method calls
    /// </summary>
    public SpyTestCommandHandler CreateSpyHandler()
    {
        return new SpyTestCommandHandler(
            MockEventStore.Object,
            MockSerializer.Object,
            MockDateTimeService.Object);
    }

    /// <summary>
    /// Creates a handler that returns null aggregate ID
    /// </summary>
    public TestCommandHandlerWithNullAggregateId CreateHandlerWithNullAggregateId()
    {
        return new TestCommandHandlerWithNullAggregateId(
            MockEventStore.Object,
            MockSerializer.Object,
            MockDateTimeService.Object);
    }

    /// <summary>
    /// Creates a handler that returns null version
    /// </summary>
    public TestCommandHandlerWithNullVersion CreateHandlerWithNullVersion()
    {
        return new TestCommandHandlerWithNullVersion(
            MockEventStore.Object,
            MockSerializer.Object,
            MockDateTimeService.Object);
    }

    /// <summary>
    /// Verifies that the DateTimeService was called exactly once
    /// </summary>
    public void VerifyDateTimeServiceCalled()
    {
        MockDateTimeService.Verify(x => x.UtcNow(), Times.Once);
    }

    /// <summary>
    /// Verifies that the serializer and event store were called correctly
    /// </summary>
    public void VerifySerializationAndStorage(EventEnvelope expectedEnvelope)
    {
        MockSerializer.Verify(x => x.Serialize(It.IsAny<EventEnvelope<TestEvent>>()), Times.Once);
        MockEventStore.Verify(x => x.AppendEventAsync(expectedEnvelope), Times.Once);
    }
}
