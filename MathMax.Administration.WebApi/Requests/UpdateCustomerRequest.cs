using System;
using System.ComponentModel.DataAnnotations;
using MathMax.EventSourcing.Api.Requests;

namespace MathMax.Administration.WebApi.Requests;

public record UpdateCustomerRequest([Required] Guid CustomerId, [Required] string NewName, [Required] int Version) : IUpdateEntityRequest;
