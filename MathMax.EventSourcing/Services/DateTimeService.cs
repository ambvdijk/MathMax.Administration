using System;

namespace MathMax.EventSourcing.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow() => DateTime.UtcNow;
    public DateTime LocalNow() => DateTime.Now;
}
