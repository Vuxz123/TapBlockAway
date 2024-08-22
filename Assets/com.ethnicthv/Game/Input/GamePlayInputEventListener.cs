using com.ethnicthv.Game.Cube;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GamePlayInputEventListener : InputEventListener
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        private CameraController _cameraController;

        public GamePlayInputEventListener(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public override void Setup()
        {
            GamePlayInputEventSystem.instance.OnPressUp += OnTap;
            GamePlayInputEventSystem.instance.OnDrag += OnDrag;
            GamePlayInputEventSystem.instance.OnZoom += OnZoom;
        }

        public override void Unplug()
        {
            GamePlayInputEventSystem.instance.OnPressUp -= OnTap;
            GamePlayInputEventSystem.instance.OnDrag -= OnDrag;
            GamePlayInputEventSystem.instance.OnZoom -= OnZoom;
        }

        private void OnTap(Touch touch)
        {
            var tapPosition = touch.position;
            
            // get the object that was tapped
            var ray = _cameraController.mainCamera.ScreenPointToRay(tapPosition);
            if (!Physics.Raycast(ray, out var hit, maxDistance: 100, layerMask: CubeManager.instance.enableLayerMask)) return;
            var tappedObject = hit.collider.gameObject;
            var temp = tappedObject.name.Split("_");
            var cube = CubeManager.instance.GetCube(int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
            if (cube != null) cube.Move();
        }
        
        private void OnDrag(Touch touch)
        {
            // drag the camera around Zero point
            var delta = touch.deltaPosition;

            var root = _cameraController.cameraRoot;
            
            //rotate the GamePlayManager.cameraRoot around the Zero point
            root.Rotate(root.up, delta.x * 0.1f, Space.World);
            root.Rotate(root.right, -delta.y * 0.1f, Space.World);
        }
        
        private void OnZoom(Touch? touch, Touch? touch1, float delta)
        {
            // zoom the camera
            _cameraController.cameraDist += delta/100;
        }
#else
        public override void Setup()
        {
        }

        public override void Unplug()
        {
        }
#endif
    }
}