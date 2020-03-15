using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Core.Events.Bus
{
    public class EventBus : IEventBus
    {
        private readonly MediatR.IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        async ValueTask IEventBus.TriggerAsync<T>(EventData<T> eventData, CancellationToken cancellationToken)
        {
            await _mediator.Publish(eventData.EventArgs, cancellationToken);
        }
    }
}