using System.Collections;
using System.Collections.Generic;
using cm.ethnicthv.Game.Input.Home;
using com.ethnicthv.Game.Data;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace com.ethnicthv.Game.Home
{
    public class CubeSelectorController : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private Transform cubeContainer;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Cube Skin Detail")] 
        [SerializeField] private Image skinLock;

        private HomeInput _homeInput;
        private TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _dragBounceTween;
        private Coroutine _dragCoroutine;
        private readonly List<CubeSkinDisplayController> _cubeSkinList = new();

        private float _currentCubeIndex;
        private bool _isDragging;

        private float _distance;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

        // Note: round the current cube index to the nearest integer
        public int currentCubeIndex => Mathf.RoundToInt(_currentCubeIndex);

        private bool _focused;

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

            var skinDatabase = SkinSelectionManager.instance.skinDatabase;
            var length = skinDatabase.length;
            for (var i = 0; i < length; i++)
            {
                var cube = Instantiate(cubePrefab, cubeContainer);
                var cubeController = cube.GetComponent<CubeSkinDisplayController>();
                cubeController.SetSkin(skinDatabase.GetSkin(i));
                _cubeSkinList.Add(cubeController);

                cube.transform.localPosition = new Vector3(i * _distance, 0, 0);
                cubeController.cubeAlpha = 0;
            }

            _cubeSkinList[0].cubeAlpha = 1;
            SetCurrentCubeIndex(0);
        }

        private void OnDisable()
        {
            _homeInput.Disable();
        }

        private void OnCurrentCubeIndexUpdated()
        {
            var current = currentCubeIndex;
            var dis2Current = current - _currentCubeIndex;
            var dir = dis2Current > 0 ? 1 : -1;
            var prev = current - dir;
            UpdateAlpha(current, prev, dis2Current, dir);
            UpdateTitle(current, prev, dis2Current, dir);
            UpdateSkinDetail(current, prev, dis2Current, dir);
            
            // Note: invoke event
            if (dis2Current == 0)
            {
                if (_focused) return;
                _focused = true;
                OnFocusSkin();
            }
            else
            {
                if (!_focused) return;
                _focused = false;
                OnUnFocusSkin();
            }
        }

        private void UpdateTitle(int current, int prev, float dis2Current, int dir)
        {
            //update title
            var skin = SkinSelectionManager.instance.skinDatabase.GetSkin(currentCubeIndex);
            
            titleText.text = skin.name;
            var c = Color.white;
            c.a = 1 - 2 * Mathf.Abs(dis2Current);
            titleText.color = c;
        }

        private void UpdateSkinDetail(int current, int prev, float dis2Current, int dir)
        {
            //update skin detail
            
            var interval = 1 - 2 * Mathf.Abs(dis2Current);
            
            // Note: update lock icon
            var maxAlpha = SaveManager.instance.skinProgressData.IsSkinUnlocked(currentCubeIndex) ? 0 : 1;
            var alpha = Mathf.Lerp(0, maxAlpha, interval);
            var c = Color.white;
            c.a = alpha;
            skinLock.color = c;
        }
        
        private void UpdateAlpha(int current, int prev, float dis2Current, int dir)
        {
            //update alpha
            var currentCube = _cubeSkinList[current];
            currentCube.cubeAlpha = 1 - Mathf.Abs(dis2Current);

            if (prev < 0 || prev >= _cubeSkinList.Count) return;
            var prevCube = _cubeSkinList[prev];
            prevCube.cubeAlpha = Mathf.Abs(dis2Current);
        }

        #region Event Listener

        public void OnSelect()
        {
            var distance = Mathf.Abs(_currentCubeIndex - currentCubeIndex);
            if (distance > 0.1f) return;
            Debug.Log("Select: " + currentCubeIndex);
        }

        /// <summary>
        /// Invoke when the _currentCubeIndex is equal to the currentCubeIndex
        /// </summary>
        private void OnFocusSkin()
        {
            var currentSkin = _cubeSkinList[currentCubeIndex];
            currentSkin.Display();
        }
        
        /// <summary>
        /// Invoke when after the _currentCubeIndex is not equal to the currentCubeIndex
        /// </summary>
        private void OnUnFocusSkin()
        {
            var currentSkin = _cubeSkinList[currentCubeIndex];
            currentSkin.Reset();
        }

        #endregion

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