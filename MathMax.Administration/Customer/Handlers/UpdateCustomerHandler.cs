using System;
using MathMax.Administration.Customer.Events;
using MathMax.EventSourcing;
using MathMax.EventSourcing.Services;

namespace MathMax.Administration.Customer.Commands;

public class UpdateCustomerHandler : CommandHandlerBase<UpdateCustomerCommand, CustomerUpdated>
{
    public UpdateCustomerHandler(IEventStore store, IEventEnvelopeSerializer serializer, IDateTimeService dateTimeService) 
        : base(store, serializer, dateTimeService)
    {
    }

    protected override CustomerUpdated CreateEvent(UpdateCustomerCommand command, DateTime timestamp)
    {
        return new CustomerUpdated(command.CustomerId, command.NewName, timestamp);
    }

    protected override Guid? GetAggregateId(UpdateCustomerCommand command)
    {
        return command.CustomerId;
    }

    protected override int? GetVersion(UpdateCustomerCommand command)
    {
        return command.Version;
    }

}