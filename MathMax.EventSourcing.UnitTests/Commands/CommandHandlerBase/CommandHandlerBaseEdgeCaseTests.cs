using System.Threading.Tasks;
using Xunit;

namespace MathMax.EventSourcing.UnitTests.Commands;

/// <summary>
/// Tests for CommandHandlerBase edge cases and special scenarios
/// </summary>
public class CommandHandlerBaseEdgeCaseTests : IClassFixture<CommandHandlerBaseTestFixture>
{
    private readonly CommandHandlerBaseTestFixture _fixture;

    public CommandHandlerBaseEdgeCaseTests(CommandHandlerBaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HandleAsync_WithNullAggregateId_CreatesEventEnvelopeWithNullAggregateId()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = _fixture.CreateTestCommand();
        var handlerWithNullAggregateId = dependencies.CreateHandlerWithNullAggregateId();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(null, 1);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        var result = await handlerWithNullAggregateId.HandleAsync(command);

        // Assert
        Assert.Null(result.AggregateId);
    }

    [Fact]
    public async Task HandleAsync_WithNullVersion_CreatesEventEnvelopeWithNullVersion()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var command = _fixture.CreateTestCommand();
        var handlerWithNullVersion = dependencies.CreateHandlerWithNullVersion();
        var serializedEnvelope = _fixture.CreateSerializedEnvelope(command.Id, null);
        dependencies.SetupSerializerMock(serializedEnvelope);

        // Act
        var result = await handlerWithNullVersion.HandleAsync(command);

        // Assert
        Assert.Null(result.Version);
    }

    [Fact]
    public void GetEventType_ReturnsCorrectEventTypeName()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act
        var eventType = dependencies.Handler.PublicGetEventType();

        // Assert
        Assert.Equal("TestEvent", eventType);
    }
}
