using System;

namespace com.ethnicthv.Game
{
    public sealed class CustomEventListener
    {
        private event Action<Event> @event;
        
        public void AddListener(Action<Event> action)
        {
            @event += action;
        }
        
        public void RemoveListener(Action<Event> action)
        {
            @event -= action;
        }

        public void OnEvent(Event obj)
        {
            @event?.Invoke(obj);
        }
    }
}