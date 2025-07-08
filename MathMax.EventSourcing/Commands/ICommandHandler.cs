using System.Threading.Tasks;

namespace MathMax.EventSourcing;

public interface ICommandHandler<TCommand, TEvent>
    where TCommand : class
    where TEvent : class
{
    Task<EventEnvelope<TEvent>> HandleAsync(TCommand command);
}
