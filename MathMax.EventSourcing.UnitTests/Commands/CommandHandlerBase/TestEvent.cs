using System;

namespace MathMax.EventSourcing.UnitTests.Commands;

public class TestEvent
{
    public Guid CommandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}
