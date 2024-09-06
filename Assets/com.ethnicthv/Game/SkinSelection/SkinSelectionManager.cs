using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    public class SkinSelectionManager : MonoBehaviour
    {
        public static SkinSelectionManager instance { get; private set; }
        
        [Header("Setup")] 
        public CubeSkinDatabase skinDatabase;
        public CameraController cameraController;
        
        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            cameraController.cameraDist = -4;
            cameraController.cameraRoot.rotation = Quaternion.Euler(30, 225, 0);
        }
        
        public void HideSkinSelection()
        {
            if (!GameManager.instance.TryChangeState(ScreenState.GamePlay, out var main)) return;
            main.Enable();
            gameObject.SetActive(false);
        }
    }
}
