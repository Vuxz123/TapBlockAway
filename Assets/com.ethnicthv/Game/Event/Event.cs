using System.Collections.Generic;

namespace com.ethnicthv.Game
{
    public abstract class Event
    {
        
        public readonly int EventId;

        protected Event(int eventId)
        {
            EventId = eventId;
        }
    }
}