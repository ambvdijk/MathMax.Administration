namespace MathMax.EventSourcing;

public interface IEventEnvelopeSerializer
{
    EventEnvelope Serialize<TEvent>(EventEnvelope<TEvent> envelope) where TEvent : class;
    EventEnvelope<TEvent> Deserialize<TEvent>(EventEnvelope envelope) where TEvent : class;
}
