#define TEST_GAMEPLAY

using System.Collections;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Input;
using com.ethnicthv.Game.Input.GamePlay;
using com.ethnicthv.Game.Map;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class GamePlayManager : MonoBehaviour
    {
        [Header("Gameplay Properties")] public string category = "Normal";
        public int level = 0;

        [Header("Input Properties")] public float tapCooldown = 0.5f;

        [Space(10)] [Header("Setup")] public MapManager mapManager;
        [SerializeField] private NewGamePlayInputListener newGamePlayInputListener;
        [SerializeField] private GamePlayInputEventSystem gamePlayInputEventSystem;
        
        public CubeManager cubeManager;
        public CameraController cameraController;

        public static GamePlayManager instance { get; private set; }

        private InputEventListener _inputEventListener;
        
        [HideInInspector] public int mapSize = 10;

        private void Awake()
        {
            instance = this;

#if !ENABLE_INPUT_SYSTEM
            gamePlayInputEventSystem = gameObject.GetComponent<GamePlayInputEventSystem>();
            _inputEventListener = new GamePlayInputEventListener(cameraController);
            _inputEventListener.Setup();
#else
            newGamePlayInputListener = gameObject.GetComponent<NewGamePlayInputListener>();
#endif
        }

        private void Start()
        {
            Debug.Log("GamePlayManager Start");

#if TEST_GAMEPLAY
            StartGame();
            cubeManager.OnAllCubeMoved += OnFinishLevel;
#endif
        }

        private void OnFinishLevel()
        {
            Debug.Log("Finish Level");
            level++;
            StartGame();
        }

        private IEnumerator ShowMap()
        {
            yield return new WaitUntil(() => mapManager.isMapLoaded);

            mapManager.ShowMap();
        }

        public void StartGame()
        {
            mapManager.LoadMap($"Assets/com.ethnicthv/R/Map/{category}/{level}.json");

            StartCoroutine(ShowMap());
        }
    }
}