#define TEST_GAMEPLAY

using System.Collections;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Input.GamePlay;
using com.ethnicthv.Game.Map;
using UnityEngine;

namespace com.ethnicthv.Game.Gameplay
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

        // <-- internal properties -->
        public static GamePlayManager instance { get; private set; }

        private InputEventListener _inputEventListener;

        [HideInInspector] public int mapSize = 10;

        public readonly GameState gameState = new GameState();
        // <-- end -->


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

        private IEnumerator StartCoroutine()
        {
            yield return new WaitUntil(() => mapManager.isMapLoaded);
            mapManager.ShowMap(() => { gameState.gamePhase = GamePhase.Playing; });
        }

        public void StartGame()
        {
            gameState.gamePhase = GamePhase.Start;
            mapManager.LoadMap($"Assets/com.ethnicthv/R/Map/{category}/{level}.json");
            StartCoroutine(StartCoroutine());
        }

        public void RestartGame()
        {
            Debug.Log("Restart Game - " + gameState.gamePhase);
            if (!gameState.isPlaying) return;
            gameState.gamePhase = GamePhase.Start;
            
            mapManager.HideMap();
            StartCoroutine(RestartCoroutine());
        }

        private IEnumerator RestartCoroutine()
        {
            yield return new WaitUntil(() => !mapManager.isMapShowed);
            yield return new WaitForSeconds(1f);
            mapManager.ShowMap(() => { gameState.gamePhase = GamePhase.Playing; });
        }

        public class GameState
        {
            public bool isPaused => gamePhase == GamePhase.Pause;

            public bool isPlaying => gamePhase == GamePhase.Playing;

            public bool isGameOver => gamePhase == GamePhase.GameOver;

            public bool isGameWin => gamePhase == GamePhase.GameWin;

            public GamePhase gamePhase { get; protected internal set; }
        }
    }

    public enum GamePhase
    {
        Start,
        Playing,
        Pause,
        GameOver,
        GameWin
    }
}