#define TEST_GAMEPLAY

using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Data;
using com.ethnicthv.Game.Input.GamePlay;
using com.ethnicthv.Game.LevelSelection;
using com.ethnicthv.Game.Map;
using UnityEngine;
using UnityEngine.Serialization;

namespace com.ethnicthv.Game.Gameplay
{
    public class GamePlayManager : MonoBehaviour
    {
        [Header("Gameplay Properties")] public int category = 1;
        public int level;

        [Header("Input Properties")] public float tapCooldown = 0.5f;

        [Space(10)] [Header("Setup")] public MapManager mapManager;
        public GameplayUIController gameplayUIController;
        [SerializeField] private GameObject disableAble;
        [SerializeField] private NewGamePlayInputListener newGamePlayInputListener;
        [SerializeField] private GamePlayInputEventSystem gamePlayInputEventSystem;
        public CubeManager cubeManager;
        public CameraController cameraController;

        // <-- internal properties -->
        public static GamePlayManager instance { get; private set; }

        private InputEventListener _inputEventListener;

        [HideInInspector] public int mapSize = 10;

        public readonly GameState State = new();
        // <-- end -->

        // <-- control properties -->
        public int currentLevel
        {
            get => level;
            set
            {
                level = value;
                SaveManager.instance.gameProgressData.currentLevel = level;
                SaveManager.instance.gameProgressData.currentCategory = category;
                gameplayUIController.UpdateLevelText(displayLevel);
                UpdateCurrentLevel();
            }
        }
        // <-- end -->

        // <-- utility properties -->
        public int displayLevel => level + 1;
        //

        private void Awake()
        {
            instance = this;
            Debug.Log("GamePlayManager Awake");
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
            category = SaveManager.instance.gameProgressData.currentCategory;
            currentLevel = SaveManager.instance.gameProgressData.currentLevel;
            cubeManager.OnAllCubeMoved += OnFinishLevel;
#endif
        }

        #region En/Dis Function

        public void Disable()
        {
            disableAble.SetActive(false);
        }

        public void Enable()
        {
            disableAble.SetActive(true);
        }

        #endregion

        private void OnFinishLevel()
        {
            Debug.Log("Finish Level");
            var isLastLevelInGroup = GameInternalSetting.IsLastLevelInGroup(category, currentLevel, out var levelGroup);
            var temp = levelGroup;
            if (isLastLevelInGroup)
            {
                SaveManager.instance.UpdateCompleteLevelGroup(category, levelGroup);
                temp++;
            }

            SaveManager.instance.UpdateGameProgress(category, temp, currentLevel);
            SaveManager.instance.SaveGameProgress();
            currentLevel++;
        }

        #region Control Function

        // <-- Start Game -->
        // Note: Start Game will load the map of current level then load the map

        // Note: This function will be called when the game is started
        // Note: Currently in Test so this function will be called in StartGame to continue the game
        //      In the future, this function will be called after Win Game or Lose Game be closed
        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitUntil(() => mapManager.isMapLoaded);
            mapManager.ShowMap(() => { State.gamePhase = GamePhase.Playing; });
        }
        
        public void PrepareGame()
        {
            State.gamePhase = GamePhase.Start;
            mapManager.LoadMap($"Assets/com.ethnicthv/R/Map/{CategoryMap[category]}/{currentLevel}.json");
            //Obsolete: be removed in the future
            StartCoroutine(StartGameCoroutine());
        }
        // <-- end -->

        private void UpdateCurrentLevel()
        {
            PrepareGame();
        }

        #endregion

        public void RestartGame()
        {
            if (!State.isPlaying) return;
            State.gamePhase = GamePhase.Start;

            mapManager.HideMap();
            StartCoroutine(RestartCoroutine());
        }

        private IEnumerator RestartCoroutine()
        {
            yield return new WaitUntil(() => !mapManager.isMapShowed);
            yield return new WaitForSeconds(1f);
            mapManager.ShowMap(() => { State.gamePhase = GamePhase.Playing; });
        }

        public void OpenLevelSelector()
        {
            if (State.gamePhase is not (GamePhase.Playing or GamePhase.Start)) return;
            if (GameManager.instance.TryChangeState(ScreenState.LevelSelection, out var main))
            {
                LevelSelectorManager.instance.ShowLevelSelector(main);
            }
        }

        public class GameState
        {
            public bool isPaused => gamePhase == GamePhase.Pause;

            public bool isPlaying => gamePhase == GamePhase.Playing;

            public bool isGameOver => gamePhase == GamePhase.GameOver;

            public bool isGameWin => gamePhase == GamePhase.GameWin;

            public GamePhase gamePhase { get; protected internal set; }
        }

        public static readonly Dictionary<int, string> CategoryMap = new()
        {
            { 0, "Easy" },
            { 1, "Normal" },
            { 2, "Hard" }
        };
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