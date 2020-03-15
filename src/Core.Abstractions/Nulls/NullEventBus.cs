using System.Threading;
using System.Threading.Tasks;

namespace Core.Events.Bus
{
    public class NullEventBus : IEventBus
    {
        ValueTask IEventBus.TriggerAsync<T>(EventData<T> eventData, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        public static NullEventBus Instance => new NullEventBus();
    }
}