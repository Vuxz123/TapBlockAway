using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    public class HomeScreenManager : MonoBehaviour
    {
        [Header("Setup")] 
        public CameraController cameraController;

        private void OnEnable()
        {
            cameraController.cameraDist = -4;
            cameraController.cameraRoot.rotation = Quaternion.Euler(30, 225, 0);
        }
    }
}
