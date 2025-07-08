using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MathMax.Administration.Customer.Commands;
using MathMax.Administration.Customer.Events;
using MathMax.Administration.WebApi.Requests;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Api.Controllers;

namespace MathMax.Administration.WebApi.Customer.Controllers;

[Route("api/customer")]
public class CreateCustomerController : CreateEntityControllerBase<CreateCustomerRequest, CreateCustomerCommand, CustomerCreated>
{
    public CreateCustomerController(
        ICommandHandler<CreateCustomerCommand, CustomerCreated> handler,
        ILogger<CreateCustomerController> logger)
        : base(logger, handler, r => r.CustomerId, "Customer")
    {
    }

    protected override CreateCustomerCommand CreateCommand(CreateCustomerRequest request)
    {
        return new CreateCustomerCommand(request.CustomerId, request.Name);
    }

    protected override string GetCreatedAtActionName()
    {
        return "Customer"; 
    }

    protected override object GetCreatedAtRouteValues(Guid aggregateId)
    {
        return new { customerId = aggregateId };
    }

    protected override IActionResult CreateSuccessResponse(EventEnvelope<CustomerCreated> eventEnvelope, Guid aggregateId)
    {
        // Since there's no GET action for retrieving customers, return Created without location header
        return Created(string.Empty, eventEnvelope);
    }
}
