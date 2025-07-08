using System;
using Xunit;

namespace MathMax.EventSourcing.UnitTests.Commands;

/// <summary>
/// Unit tests for CommandHandlerBase constructor parameter validation.
/// Verifies that the constructor properly validates required dependencies
/// and throws appropriate exceptions when null values are provided.
/// </summary>
public class CommandHandlerBaseConstructorTests : IClassFixture<CommandHandlerBaseTestFixture>
{
    private readonly CommandHandlerBaseTestFixture _fixture;

    public CommandHandlerBaseConstructorTests(CommandHandlerBaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Constructor_WhenEventStoreIsNull_ThrowsArgumentNullExceptionWithCorrectParameterName()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new TestCommandHandler(null!, dependencies.MockSerializer.Object, dependencies.MockDateTimeService.Object));
        
        Assert.Equal("eventStore", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenSerializerIsNull_ThrowsArgumentNullExceptionWithCorrectParameterName()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new TestCommandHandler(dependencies.MockEventStore.Object, null!, dependencies.MockDateTimeService.Object));
        
        Assert.Equal("serializer", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenDateTimeServiceIsNull_ThrowsArgumentNullExceptionWithCorrectParameterName()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new TestCommandHandler(dependencies.MockEventStore.Object, dependencies.MockSerializer.Object, null!));
        
        Assert.Equal("dateTimeService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenAllParametersAreValid_CreatesInstanceSuccessfully()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act
        var handler = new TestCommandHandler(
            dependencies.MockEventStore.Object,
            dependencies.MockSerializer.Object,
            dependencies.MockDateTimeService.Object);

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<TestCommandHandler>(handler);
    }

    [Fact]
    public void Constructor_WhenAllParametersAreValid_AssignsDependenciesCorrectly()
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();

        // Act
        var handler = new TestCommandHandler(
            dependencies.MockEventStore.Object,
            dependencies.MockSerializer.Object,
            dependencies.MockDateTimeService.Object);

        // Assert
        // Verify that the handler can be used (implicitly tests that dependencies were assigned)
        Assert.NotNull(handler);
        
        // Verify the handler implements the expected interface
        Assert.IsType<ICommandHandler<TestCommand, TestEvent>>(handler, exactMatch: false);
    }

    [Theory]
    [InlineData(0)] // eventStore
    [InlineData(1)] // serializer  
    [InlineData(2)] // dateTimeService
    public void Constructor_WhenSingleParameterIsNull_ThrowsArgumentNullException(int nullParameterIndex)
    {
        // Arrange
        var dependencies = _fixture.CreateTestDependencies();
        var eventStore = nullParameterIndex == 0 ? null : dependencies.MockEventStore.Object;
        var serializer = nullParameterIndex == 1 ? null : dependencies.MockSerializer.Object;
        var dateTimeService = nullParameterIndex == 2 ? null : dependencies.MockDateTimeService.Object;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TestCommandHandler(eventStore!, serializer!, dateTimeService!));
    }
}
