using System;
using System.Collections;
using System.Collections.Generic;
using cm.ethnicthv.Game.Input.Home;
using com.ethnicthv.Game.Cube;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    public class CubeSelectorController : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private Transform cubeContainer;
        [SerializeField] private GameObject cubePrefab;

        private HomeInput _homeInput;
        private TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _dragBounceTween;
        private Coroutine _dragCoroutine;
        private List<CubeController> _cubeSkinList = new();

        private float _currentCubeIndex;
        private bool _isDragging;

        private float _distance;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

        // Note: round the current cube index to the nearest integer
        public int currentCubeIndex => Mathf.RoundToInt(_currentCubeIndex);

        private void SetCurrentCubeIndex(float value)
        {
            _currentCubeIndex = value;
            OnCurrentCubeIndexUpdated();
        }

        private void Awake()
        {
            _homeInput = new HomeInput();
            _homeInput.Home.Drag.started += _ => { OnDragStart(); };
            _homeInput.Home.Drag.canceled += _ => { OnDragEnd(); };
        }

        private void OnEnable()
        {
            _homeInput.Enable();
        }

        private void Start()
        {
            var cam = Camera.main!;
            var camDist = Vector3.Distance(cam.transform.position, Vector3.zero);
            var pointA = cam.ScreenPointToRay(new Vector3(0, 0, 0)).GetPoint(camDist).x;
            var pointB = cam.ScreenPointToRay(new Vector3(Screen.width, 0, 0)).GetPoint(camDist).x;

            _distance = (pointB - pointA) / 0.75f;
            Debug.Log("Distance: " + _distance);

            for (var i = 0; i < 5; i++)
            {
                var cube = Instantiate(cubePrefab, cubeContainer);
                var cubeController = cube.GetComponent<CubeController>();
                _cubeSkinList.Add(cubeController);

                cube.transform.localPosition = new Vector3(i * _distance, 0, 0);
                cubeController.cubeAlpha = 0;
            }
            
            _cubeSkinList[0].cubeAlpha = 1;
        }

        private void OnDisable()
        {
            _homeInput.Disable();
        }

        private void OnCurrentCubeIndexUpdated()
        {
            UpdateAlpha();
        }

        private void UpdateAlpha()
        {
            var current = currentCubeIndex;
            var dis2Current = current - _currentCubeIndex;
            var dir = dis2Current > 0 ? 1 : -1;
            var prev = current - dir;
            //update alpha
            var currentCube = _cubeSkinList[current];
            currentCube.cubeAlpha = 1 - Mathf.Abs(dis2Current);
            
            if (prev >= 0 && prev < _cubeSkinList.Count)
            {
                var prevCube = _cubeSkinList[prev];
                prevCube.cubeAlpha = Mathf.Abs(dis2Current);
            }
        }

        #region Input Listeners

        private void OnDragStart()
        {
            _isDragging = true;

            if (_dragBounceTween != null && _dragBounceTween.IsPlaying())
            {
                _dragBounceTween.Kill();
                _dragBounceTween = null;
            }

            SetCurrentCubeIndex(-cubeContainer.localPosition.x / _distance);

            _dragCoroutine = StartCoroutine(OnDragCoroutine());
        }

        private void OnDragEnd()
        {
            _isDragging = false;
            if (_dragCoroutine != null) StopCoroutine(_dragCoroutine);

            _dragBounceTween = cubeContainer.DOLocalMove(new Vector3(-currentCubeIndex * _distance, 0, 0), 1f)
                .SetEase(Ease.OutCirc)
                .OnUpdate(() => SetCurrentCubeIndex(-cubeContainer.localPosition.x / _distance))
                .OnComplete(() => _currentCubeIndex = currentCubeIndex);
        }

        private IEnumerator OnDragCoroutine()
        {
            while (_isDragging)
            {
                var delta = _homeInput.Home.DragDelta.ReadValue<Vector2>().x / Screen.width * 1.4f;
                OnDrag(delta);
                yield return _waitForFixedUpdate;
            }
        }

        private void OnDrag(float deltaX)
        {
            SetCurrentCubeIndex(Mathf.Clamp(_currentCubeIndex - deltaX, -0.5f, _cubeSkinList.Count - 0.5f));
            cubeContainer.localPosition = new Vector3(-_currentCubeIndex * _distance, 0, 0);
        }

        #endregion
    }
}