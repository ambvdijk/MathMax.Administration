using System;

namespace MathMax.EventSourcing.Entity;

public class Event
{
    public required byte[] UniqueId { get; set; }
    public required string ReadableId { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
    public Guid? AggregateId { get; set; }
    public int? Version { get; set; }
    public required DateTime Timestamp { get; set; }
}
