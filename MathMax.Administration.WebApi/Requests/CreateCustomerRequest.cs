using System;

namespace MathMax.Administration.WebApi.Requests;

public record CreateCustomerRequest(Guid CustomerId, string Name);
