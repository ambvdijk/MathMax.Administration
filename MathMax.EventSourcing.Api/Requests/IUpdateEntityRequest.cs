namespace MathMax.EventSourcing.Api.Requests;

public interface IUpdateEntityRequest
{
    int Version { get; }
}