using com.ethnicthv.Game.Cube.CubeSkin;
using com.ethnicthv.Game.Gameplay;
using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    public class SkinSelectionManager : MonoBehaviour
    {
        public static SkinSelectionManager instance { get; private set; }

        [Header("Setup")] public DisableAble disableAble;
        public CubeSkinDatabase skinDatabase;
        public CameraController cameraController;
        
        private void Awake()
        {
            instance = this;
            disableAble.onEnable += () =>
            {
                Debug.Log("SkinSelectionManager Enable");
                cameraController.cameraDist = -4;
                cameraController.cameraRoot.rotation = Quaternion.Euler(30, 225, 0);
            };
        }
        
        public void HideSkinSelection()
        {
            if (!GameManager.instance.TryChangeState(ScreenState.GamePlay, out var main)) return;
            main.Enable();
            disableAble.Disable();
        }
        
        public void ShowSkinSelection(GamePlayManager main)
        {
            disableAble.Enable();
        }
    }
}
