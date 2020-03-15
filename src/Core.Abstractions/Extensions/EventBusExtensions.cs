using System.Threading;
using System.Threading.Tasks;
using Core.Events.Bus;

namespace Core.Events
{
    public static class EventBusExtensions
    {
        public static ValueTask TriggerAsync<T>(this IEventBus @this, T eventArgs, CancellationToken cancellationToken = default)
        {
            return @this.TriggerAsync(new EventData<T>(eventArgs), cancellationToken);
        }
    }
}