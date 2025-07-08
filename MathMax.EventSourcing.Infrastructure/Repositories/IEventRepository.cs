using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MathMax.EventSourcing.Infrastructure.Repositories;

public interface IEventRepository
{
    Task<bool> AppendEventAsync(EventEnvelope envelope);

}
