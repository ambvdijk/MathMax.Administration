using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MathMax.Administration.Customer.Commands;
using MathMax.Administration.Customer.Events;
using MathMax.Administration.WebApi.Requests;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Api.Controllers;



namespace MathMax.Administration.WebApi.Customer.Controllers;

[ApiController]
[Route("api/customer")]
[Produces("application/json")]
public class UpdateCustomerController(ICommandHandler<UpdateCustomerCommand, CustomerUpdated> handler, ILogger<UpdateCustomerController> logger) :
    UpdateEntityControllerBase<UpdateCustomerRequest, UpdateCustomerCommand, CustomerUpdated>(logger, handler, request => request.CustomerId, "Customer")

{
    protected override UpdateCustomerCommand CreateCommand(UpdateCustomerRequest request)
    {
        return new UpdateCustomerCommand(
            request.CustomerId,
            request.NewName,
            request.Version);
    }

    protected override Task<string?> ValidateRequestAsync(UpdateCustomerRequest request)
    {
        return Task.FromResult(ValidateRequestSync(request));
    }

    private static string? ValidateRequestSync(UpdateCustomerRequest request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            return "CustomerId cannot be empty.";
        }

        if (string.IsNullOrWhiteSpace(request.NewName))
        {
            return "NewName cannot be empty.";
        }

        return null;
    }
}
