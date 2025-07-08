using System;
using System.Threading.Tasks;
using MathMax.Administration.Customer.Events;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Services;

namespace MathMax.Administration.Customer.Commands;

public class CreateCustomerHandler : CommandHandlerBase<CreateCustomerCommand, CustomerCreated>
{
    public CreateCustomerHandler(IEventStore store, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService) 
        : base(store, serializer, dateTimeService)
    {
    }

    protected override CustomerCreated CreateEvent(CreateCustomerCommand command, DateTime timestamp)
    {
        return new CustomerCreated(command.CustomerId, command.Name, timestamp);
    }

    protected override Guid? GetAggregateId(CreateCustomerCommand command)
    {
        return command.CustomerId;
    }

    protected override int? GetVersion(CreateCustomerCommand command)
    {
        return 1; // First version for new aggregate
    }
}