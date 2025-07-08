using System;

namespace MathMax.Administration.Customer.Commands;

public record CreateCustomerCommand(Guid CustomerId, string Name);
