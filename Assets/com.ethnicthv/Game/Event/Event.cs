namespace com.ethnicthv.Game
{
    public abstract class Event
    {
        public readonly int EventId;

        protected Event(int eventId)
        {
            this.EventId = eventId;
        }
    }
}