using System.Threading.Tasks;
using Xunit;

namespace MathMax.EventSourcing.UnitTests.Commands;

/// <summary>
/// Tests for CommandHandlerBase.HandleAsync method core functionality
/// </summary>
public class CommandHandlerBaseHandleAsyncTests : IClassFixture<CommandHandlerBaseTestFixture>
{
    private readonly CommandHandlerBaseTestFixture _fixture;

    public CommandHandlerBaseHandleAsyncTests(CommandHandlerBaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_CreatesEventEnvelopeWithCorrectProperties()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = CommandHandlerBaseTestFixture.CreateTestCommand();
        var expectedEvent = _fixture.CreateExpectedEvent(command);
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        var result = await dependencies.Handler.HandleAsync(command);

        // Assert
        AssertEventEnvelopeProperties(result, expectedEvent, command);
    }

    [Fact]
    public async Task HandleAsync_GeneratesUlidForEvent()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = CommandHandlerBaseTestFixture.CreateTestCommand();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        var result = await dependencies.Handler.HandleAsync(command);

        // Assert
        AssertValidUlidGeneration(result);
    }

    [Fact]
    public async Task HandleAsync_CallsDateTimeServiceForTimestamp()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = CommandHandlerBaseTestFixture.CreateTestCommand();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        await dependencies.Handler.HandleAsync(command);

        // Assert
        dependencies.VerifyDateTimeServiceCalled();
    }

    [Fact]
    public async Task HandleAsync_SerializesAndStoresEventEnvelope()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = CommandHandlerBaseTestFixture.CreateTestCommand();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        await dependencies.Handler.HandleAsync(command);

        // Assert
        dependencies.VerifySerializationAndStorage(serializedEnvelope);
    }

    [Fact]
    public async Task HandleAsync_CallsAbstractMethodsCorrectly()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = CommandHandlerBaseTestFixture.CreateTestCommand();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);
        var spyHandler = dependencies.CreateSpyHandler();

        // Act
        await spyHandler.HandleAsync(command);

        // Assert
        AssertAbstractMethodsCalled(spyHandler, command);
    }

    private void AssertEventEnvelopeProperties(EventEnvelope<TestEvent> result, TestEvent expectedEvent, TestCommand command)
    {
        Assert.NotNull(result);
        Assert.Equal("TestEvent", result.EventType);
        Assert.Equal(command.Id, result.AggregateId);
        Assert.Equal(1, result.Version);
        Assert.Equal(_fixture.TestTimestamp, result.Timestamp);
        Assert.Equal(expectedEvent.CommandId, result.Payload.CommandId);
        Assert.Equal(expectedEvent.Name, result.Payload.Name);
        Assert.Equal(expectedEvent.ProcessedAt, result.Payload.ProcessedAt);
    }

    private static void AssertValidUlidGeneration(EventEnvelope<TestEvent> result)
    {
        Assert.NotNull(result.UniqueId);
        Assert.True(result.UniqueId.Length > 0);
        Assert.NotNull(result.ReadableId);
        Assert.NotEmpty(result.ReadableId);
        
        // Verify it's a valid ULID format (26 characters)
        Assert.Equal(26, result.ReadableId.Length);
    }

    private void AssertAbstractMethodsCalled(SpyTestCommandHandler spyHandler, TestCommand command)
    {
        Assert.True(spyHandler.CreateEventCalled);
        Assert.True(spyHandler.GetAggregateIdCalled);
        Assert.True(spyHandler.GetVersionCalled);
        Assert.True(spyHandler.GetEventTypeCalled);
        Assert.Equal(command, spyHandler.LastCommand);
        Assert.Equal(_fixture.TestTimestamp, spyHandler.LastTimestamp);
    }
}
