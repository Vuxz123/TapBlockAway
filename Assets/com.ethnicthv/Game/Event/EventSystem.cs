using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem instance
        {
            get;
            private set;
        }
        
        private readonly Dictionary<Type, CustomEventListener> _listeners = new();
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void RegisterListener<T>(Action<T> listener) where T : Event
        {
            var eventType = typeof(T);
            if (_listeners.TryGetValue(eventType, out var listenerContainer))
            {
                listenerContainer.AddListener((Action<Event>) listener);
            }
            else
            {
                _listeners.Add(eventType, new CustomEventListener());
                _listeners[eventType].AddListener((Action<Event>) listener);
            }
        }
        
        public void UnregisterListener<T>(Action<T> listener) where T : Event
        {
            var eventType = typeof(T);
            if (_listeners.TryGetValue(eventType, out var listenerContainer))
            {
                listenerContainer.RemoveListener((Action<Event>) listener);
            }
        }
        
        public void TriggerEvent<T>(T eventObject) where T : Event
        {
            var eventType = typeof(T);
            if (_listeners.TryGetValue(eventType, out var listenerContainer))
            {
                listenerContainer.OnEvent(eventObject);
            }
        }
    }
}