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
        
        private readonly Dictionary<int, Action<Event>> _listenerIds = new();
        private readonly Dictionary<Type, CustomEventListener> _listeners = new();
        
        private int _listenerIdCounter = 0;
        
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
        
        public int RegisterListener<T>(Action<T> listener) where T : Event
        {
            Debug.Log("Registering listener for " + typeof(T));
            var eventType = typeof(T);
            if (!_listeners.ContainsKey(typeof(T)))
                _listeners.Add(eventType, new CustomEventListener());
            var castedListener = new Action<Event>(e => listener(e as T));
            var listenerId = _listenerIdCounter;
            _listenerIds.Add(listenerId, castedListener);
            _listeners[eventType].AddListener(e => listener(e as T));
            _listenerIdCounter++;
            Debug.Log("Listener registered with id: " + listenerId);
            return listenerId;
        }
        
        public void UnregisterListener<T>(int listenerId) where T : Event
        {
            var eventType = typeof(T);
            if (!_listeners.TryGetValue(eventType, out var listenerContainer)) return;
            var listener = _listenerIds[listenerId];
            listenerContainer.RemoveListener(listener);
        }
        
        public void TriggerEvent<T>(T eventObject) where T : Event
        {
            Debug.Log("Triggering event: " + typeof(T));
            var eventType = typeof(T);
            if (_listeners.TryGetValue(eventType, out var listenerContainer))
            {
                Debug.Log("Listener container found for " + eventType);
                listenerContainer.OnEvent(eventObject);
            }
        }
    }
}