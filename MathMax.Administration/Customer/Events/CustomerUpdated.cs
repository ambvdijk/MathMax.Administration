using System;

namespace MathMax.Administration.Customer.Events;

public record CustomerUpdated(Guid CustomerId, string Name, DateTime Timestamp);
