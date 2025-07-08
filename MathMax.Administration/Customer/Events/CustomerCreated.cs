using System;

namespace MathMax.Administration.Customer.Events;

public record CustomerCreated(Guid CustomerId, string Name, DateTime Timestamp);
