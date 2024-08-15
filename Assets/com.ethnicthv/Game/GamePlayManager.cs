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
        [SerializeField] private MapManager mapManager;
        public Transform cameraRoot;
        
        public static GamePlayManager instance { get; private set; }
        
        private InputEventListener _inputEventListener;
        
        [HideInInspector] public int mapSize = 10;
        [HideInInspector] public float cameraDistance = 10;
        public Vector3 cameraVector => new Vector3(0,0,-cameraDistance);
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            Debug.Log("GamePlayManager Start");
            _inputEventListener = new GamePlayInputEventListener(Camera.main);
            _inputEventListener.Setup();
            
            mapManager.LoadMap("Assets/com.ethnicthv/R/Map/map1.json");
        }
        
    }
}