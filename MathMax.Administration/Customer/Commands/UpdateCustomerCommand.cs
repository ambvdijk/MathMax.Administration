using System;

namespace MathMax.Administration.Customer.Commands;

public record UpdateCustomerCommand(Guid CustomerId, string NewName, int Version);
