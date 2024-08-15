using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.ethnicthv.Game
{
    public class GamePlayInputEventSystem: MonoBehaviour
    {
        public float dragThreshold = 0.1f;
        
        public static GamePlayInputEventSystem instance { get; private set; }
        
        // <-- state -->
        private bool _isDragging;
        
        private Vector2 _startPosition;
        private Vector2 _endPosition;
        
        private float _holdTime;
        
        private float _tapTime;
        // <-- end -->
        
        // <-- event -->
        public event Action<Touch> OnPressDown = touch => { };
        public event Action<Touch> OnPressUp = touch => { };
        public event Action<Touch> OnHold = touch => { };
        public event Action<Touch> OnDragStart = touch => { };
        public event Action<Touch> OnDrag = touch => { };
        public event Action<Touch> OnDragEnd = touch => { };

        private Dictionary<int, Action<Touch>> _touchPhase;
        private void Awake()
        {
            instance = this;
            
            _touchPhase = new Dictionary<int, Action<Touch>>
            {
                {0, touch => // TouchPhase.Began
                {
                    _startPosition = touch.position;
                    OnPressDown(touch);
                }},
                {1, touch => // TouchPhase.Moved
                {
                    if (touch.deltaPosition.magnitude < dragThreshold) return;
                    if (!_isDragging)
                    {
                        _isDragging = true;
                        OnDragStart(touch);
                        return;
                    }
                    OnDrag(touch);
                }},
                {2, touch => // TouchPhase.Stationary
                {
                    if (_isDragging) return;
                    _holdTime += Time.deltaTime;
                    OnHold(touch);
                }},
                {3, touch => // TouchPhase.Ended
                {
                    _endPosition = touch.position;
                    if (_isDragging)
                    {
                        OnDragEnd(touch);
                        _isDragging = false;
                        return;
                    }
                    OnPressUp(touch);
                }},
                {4, _ => // TouchPhase.Canceled
                {
                    Debug.Log("Touch canceled");
                }}
            };
        }
        
        private void Update()
        {
            if (Input.touchCount <= 0) return;
            var touch = Input.GetTouch(0);
            if (_touchPhase.TryGetValue((int)touch.phase, out var action))
            {
                action(touch);
            }
            else
            {
                Debug.LogError("Touch phase not found");
            }
        }
    }
}