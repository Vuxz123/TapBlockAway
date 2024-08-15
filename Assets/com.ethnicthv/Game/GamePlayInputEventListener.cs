using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Map;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GamePlayInputEventListener : InputEventListener
    {
        private readonly Camera _camera;

        public GamePlayInputEventListener(Camera camera)
        {
            _camera = camera;
        }

        public override void Setup()
        {
            GamePlayInputEventSystem.instance.OnPressUp += OnTap;
            GamePlayInputEventSystem.instance.OnDrag += OnDrag;
        }

        public override void Unplug()
        {
            GamePlayInputEventSystem.instance.OnPressUp -= OnTap;
            GamePlayInputEventSystem.instance.OnDrag -= OnDrag;
        }

        private void OnTap(Touch touch)
        {
            var tapPosition = touch.position;
            
            // get the object that was tapped
            var ray = _camera.ScreenPointToRay(tapPosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            var tappedObject = hit.collider.gameObject;
            var temp = tappedObject.name.Split("_");
            var cube = CubeManager.instance.GetCube(int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
            if (cube != null) cube.Move();
        }
        
        private void OnDrag(Touch touch)
        {
            // drag the camera around Zero point
            var delta = touch.deltaPosition;

            var root = GamePlayManager.instance.cameraRoot;
            
            //rotate the GamePlayManager.cameraRoot around the Zero point
            root.Rotate(root.up, delta.x * 0.1f, Space.World);
            root.Rotate(root.right, -delta.y * 0.1f, Space.World);
        }
    }
}