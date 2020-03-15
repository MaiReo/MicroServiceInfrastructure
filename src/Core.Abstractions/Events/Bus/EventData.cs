using System;
using Core.PersistentStore;

namespace Core.Events
{
    public class EventData<TEventArgs>
    {
        public EventData(TEventArgs eventArgs)
        {
            EventArgs = eventArgs;

            EventTime = Clock.Now;
        }

        public TEventArgs EventArgs { get; }
        public DateTimeOffset EventTime { get; }
    }
}