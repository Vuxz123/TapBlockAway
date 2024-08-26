using System.Collections;
using com.ethnicthe.Game.Input.GamePlay;
using com.ethnicthv.Game.Cube;
using UnityEngine;

namespace com.ethnicthv.Game.Input.GamePlay
{
    public class NewGamePlayInputListener : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;

        private GamePlayInput _gamePlayInput;

        // <-- drag -->
        private Coroutine _dragCoroutine;
        // <-- end -->

        // <-- zoom -->
        private Coroutine _zoomCoroutine;
        private float _previousZoomDistance;
        // <-- end -->

        // <-- state -->
        private bool _isZooming;
        private bool _isDrag;
        // <-- end -->

        private void Awake()
        {
            _gamePlayInput = new GamePlayInput();
            
            //Note: calling OnDragEnd and OnZoomStart when touch1 is pressed
            //      is necessary to stop the drag coroutines when it is
            //      starting zooming
            _gamePlayInput.GamePlay.Touch1Pressed.started += _ =>
            {
                OnDragEnd();
                OnZoomStart();
            };
            _gamePlayInput.GamePlay.Touch1Pressed.canceled += _ => { OnZoomEnd(); };
            _gamePlayInput.GamePlay.Touch0Pressed.started += _ => { OnDragStart(); };
            
            //Note: calling OnDragEnd and OnZoomEnd when touch0 is released
            //      is necessary to stop the zoom coroutines when touch0 is
            //      released first.
            _gamePlayInput.GamePlay.Touch0Pressed.canceled += _ =>
            {
                OnDragEnd();
                OnZoomEnd();
            };
            
            _gamePlayInput.GamePlay.Touch0Click.performed += _ => { OnTap(); };
        }

        private void OnEnable()
        {
            _gamePlayInput.Enable();
        }

        private void OnDisable()
        {
            _gamePlayInput.Disable();
        }

        #region Tap
        
        private void OnTap()
        {
            var tapPosition = _gamePlayInput.GamePlay.Touch0Position.ReadValue<Vector2>();
            // get the object that was tapped
            var ray = cameraController.mainCamera.ScreenPointToRay(tapPosition);
            if (!Physics.Raycast(ray, out var hit, maxDistance: 100, layerMask: CubeManager.instance.enableLayerMask)) return;
            var tappedObject = hit.collider.gameObject;
            var temp = tappedObject.name.Split("_");
            var cube = CubeManager.instance.GetCube(int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
            if (cube != null) cube.Move();
        }

        #endregion

        #region Zoom

        private void OnZoomEnd()
        {
            _isZooming = false;
            if(_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
            _zoomCoroutine = null;
        }

        private void OnZoomStart()
        {
            _isZooming = true;
            _zoomCoroutine = StartCoroutine(OnZoomCoroutine());
        }

        private IEnumerator OnZoomCoroutine()
        {
            while (_isZooming)
            {
                var pos1 = _gamePlayInput.GamePlay.Touch0Position.ReadValue<Vector2>();
                var pos2 = _gamePlayInput.GamePlay.Touch1Position.ReadValue<Vector2>();
                
                var distance = Vector2.Distance(pos1, pos2);
                if (Mathf.Approximately(_previousZoomDistance, 0))
                {
                    _previousZoomDistance = distance;
                    yield return null;
                }

                var delta = distance - _previousZoomDistance;
                _previousZoomDistance = distance;
                OnZoom(Mathf.Clamp(delta, -20,20));
                yield return null;
            }
        }

        private void OnZoom(float delta)
        {
            Debug.Log("OnZoom - " + delta);
            cameraController.cameraDist += delta / 100;
        }

        #endregion

        #region Drag

        private void OnDragStart()
        {
            _isDrag = true;
            _dragCoroutine = StartCoroutine(OnDragCoroutine());
        }

        private void OnDragEnd()
        {
            _isDrag = false;
            if (_dragCoroutine != null) StopCoroutine(_dragCoroutine);
            _dragCoroutine = null;
        }

        private IEnumerator OnDragCoroutine()
        {
            while (_isDrag)
            {
                var delta = _gamePlayInput.GamePlay.Touch0Delta.ReadValue<Vector2>();
                OnDrag(delta);
                yield return null;
            }
        }

        private void OnDrag(Vector2 delta)
        {
            var root = cameraController.cameraRoot;
            root.Rotate(root.up, delta.x * 0.1f, Space.World);
            root.Rotate(root.right, -delta.y * 0.1f, Space.World);
        }

        #endregion
    }
}