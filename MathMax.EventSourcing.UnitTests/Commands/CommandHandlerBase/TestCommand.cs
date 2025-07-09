using System;
using MathMax.EventSourcing;

namespace MathMax.EventSourcing.UnitTests.Commands;

// Test command and event classes
public class TestCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
