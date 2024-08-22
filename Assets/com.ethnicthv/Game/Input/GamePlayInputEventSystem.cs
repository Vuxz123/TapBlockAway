
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GamePlayInputEventSystem : MonoBehaviour
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        public float maxZoomDelta = 20;
        public float dragThreshold = 0.1f;

        public static GamePlayInputEventSystem instance { get; private set; }

        // <-- state -->
        private bool _isDragging;
        private bool _isZooming;

        // zoom
        private float _previousZoomDistance;

        // hold
        private float _holdTime;

        //tap
        private float _tapTime;
        // <-- end -->

        // <-- event -->
        public event Action<Touch> OnPressDown = _ => { };
        public event Action<Touch> OnPressUp = _ => { };
        public event Action<Touch> OnHold = _ => { };
        public event Action<Touch> OnDragStart = _ => { };
        public event Action<Touch> OnDrag = _ => { };
        public event Action<Touch> OnDragEnd = _ => { };
        public event Action<Touch?, Touch?, float> OnZoom = (_, _, _) => { };
        // <-- end -->

        private Dictionary<int, Action<Touch>> _touchPhase;
        private Dictionary<int, Action<Touch, Touch>> _zoomPhase;

        private void Awake()
        {
            instance = this;

            _touchPhase = new Dictionary<int, Action<Touch>>
            {
                {
                    0, touch => // TouchPhase.Began
                    {
                        OnPressDown(touch);
                    }
                },
                {
                    1, touch => // TouchPhase.Moved
                    {
                        if (touch.deltaPosition.magnitude < dragThreshold) return;
                        if (!_isDragging)
                        {
                            _isDragging = true;
                            OnDragStart(touch);
                            return;
                        }

                        OnDrag(touch);
                    }
                },
                {
                    2, touch => // TouchPhase.Stationary
                    {
                        if (_isDragging) return;
                        _holdTime += Time.deltaTime;
                        OnHold(touch);
                    }
                },
                {
                    3, touch => // TouchPhase.Ended
                    {
                        if (_isZooming)
                        {
                            _isZooming = false;
                            if (_previousZoomDistance == 0) return;
                            _previousZoomDistance = 0;
                            return;
                        }

                        if (_isDragging)
                        {
                            OnDragEnd(touch);
                            _isDragging = false;
                            return;
                        }

                        OnPressUp(touch);
                    }
                },
                {
                    4, _ => // TouchPhase.Canceled
                    {
                        _isDragging = false;
                        _isZooming = false;
                        Debug.Log("Touch canceled");
                    }
                }
            };

            _zoomPhase = new Dictionary<int, Action<Touch, Touch>>
            {
                {
                    0, (_, _) => // TouchPhase.Began
                    {
                    }
                },
                {
                    1, (touch1, touch2) => // TouchPhase.Moved
                    {
                        _isZooming = true;

                        var distance = Vector2.Distance(touch1.position, touch2.position);

                        if (Mathf.Approximately(_previousZoomDistance, 0))
                        {
                            _previousZoomDistance = distance;
                            return;
                        }
                        
                        var delta = distance - _previousZoomDistance;
                        _previousZoomDistance = distance;
                        OnZoom(touch1, touch2, Mathf.Clamp(delta, -maxZoomDelta, maxZoomDelta));
                    }
                },
                {
                    2, (_, _) => // TouchPhase.Ended
                    {
                        _previousZoomDistance = 0;
                    }
                },
                {
                    3, (_, _) => // TouchPhase.Stationary
                    {
                    }
                },
                {
                    4, (_, _) => // TouchPhase.Canceled
                    {
                    }
                },
            };
        }

        private void Update()
        {
#if UNITY_EDITOR

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                OnZoom(null, null, Input.mouseScrollDelta.y * 100);
            }

#endif
            if (Input.touchCount <= 0) return;
            switch (Input.touchCount)
            {
                case 1:
                    var touch = Input.GetTouch(0);
                    if (_touchPhase.TryGetValue((int)touch.phase, out var action))
                    {
                        action(touch);
                    }
                    else
                    {
                        Debug.LogError("Touch phase not found");
                    }

                    break;
                case >= 2:
                    var touch1 = Input.GetTouch(0);
                    var touch2 = Input.GetTouch(1);

                    if (_zoomPhase.TryGetValue((int)touch2.phase, out var zoomAction))
                    {
                        zoomAction(touch1, touch2);
                    }

                    break;
            }
        }
#endif
    }
}