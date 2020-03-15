using System.Threading;
using System.Threading.Tasks;

namespace Core.Events.Bus
{
    public interface IEventBus
    {
        ValueTask TriggerAsync<T>(EventData<T> eventData, CancellationToken cancellationToken = default);
    }
}