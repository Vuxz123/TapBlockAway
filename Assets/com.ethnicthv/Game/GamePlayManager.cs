using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Map;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GamePlayManager: MonoBehaviour
    {
        [Header("Input Properties")]
        public float tapCooldown = 0.5f;
        
        [Space(10)]
        [Header("Setup")]
        public MapManager mapManager;
        public CameraController cameraController;
        
        public static GamePlayManager instance { get; private set; }
        public Transform cameraRoot => cameraController.cameraRoot;
        public float cameraDistance { set => cameraController.cameraDist = value; }

        private InputEventListener _inputEventListener;
        
        [HideInInspector] public int mapSize = 10;
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            Debug.Log("GamePlayManager Start");
            _inputEventListener = new GamePlayInputEventListener(cameraController);
            _inputEventListener.Setup();
            
            mapManager.LoadMap("Assets/com.ethnicthv/R/Map/map1.json");
            
            StartCoroutine(ShowMap());
        }
        
        private IEnumerator ShowMap()
        {
            yield return new WaitUntil(() => mapManager.isMapLoaded);
            
            mapManager.ShowMap();
        }
        
    }
}