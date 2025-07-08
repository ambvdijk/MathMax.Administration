using System;

namespace MathMax.EventSourcing.Services;

public interface IDateTimeService
{
    DateTime UtcNow();
    DateTime LocalNow();
}