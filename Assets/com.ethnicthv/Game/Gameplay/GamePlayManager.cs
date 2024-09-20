#define TEST_GAMEPLAY

using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Cube;
using com.ethnicthv.Game.Data;
using com.ethnicthv.Game.GameEnd;
using com.ethnicthv.Game.Home;
using com.ethnicthv.Game.Impl;
using com.ethnicthv.Game.Input.GamePlay;
using com.ethnicthv.Game.LevelSelection;
using com.ethnicthv.Game.Map;
using DG.Tweening;
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
        [SerializeField] private DisableAble disableAble;
        [SerializeField] private NewGamePlayInputListener newGamePlayInputListener;
        [SerializeField] private GamePlayInputEventSystem gamePlayInputEventSystem;
        public CubeManager cubeManager;
        public CameraController cameraController;
        public GameControlTutorialController gameControlTutorialController;
        public AnimatedInOutController animatedInOutController;

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
        // <-- end -->

        // <-- cache -->
        public float cameraDistCache;
        // <-- end -->

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
            disableAble.onDisable += () =>
            {
                Debug.Log("GamePlayManager Disable");
                cameraDistCache = cameraController.cameraDist;
            };

            disableAble.onEnable += () =>
            {
                Debug.Log("GamePlayManager Enable");
                cameraController.cameraDist = cameraDistCache;
            };
        }

        private void Start()
        {
            Debug.Log("GamePlayManager Start");

            CubeManager.instance.currentSkin =
                SkinSelectionManager.instance.skinDatabase.GetSkin(SaveManager.instance.playerData.currentSkin);

#if TEST_GAMEPLAY
            category = SaveManager.instance.gameProgressData.currentCategory;
            currentLevel = SaveManager.instance.gameProgressData.currentLevel;
            cubeManager.OnAllCubeMoved += OnFinishLevel;
#endif
        }

        #region En/Dis Function

        public void Disable()
        {
            disableAble.Disable();
        }

        public void Enable()
        {
            disableAble.Enable();
        }

        public void AnimatedDisable(Action onComplete = null)
        {
            animatedInOutController.AnimatedOut(onComplete);
        }

        public void AnimatedEnable()
        {
            ColorUtility.TryParseHtmlString("#6CAB6D", out var color);
            animatedInOutController.AnimatedIn(color);
        }

        #endregion

        private void OnFinishLevel()
        {
            Debug.Log("Finish Level");
            
            State.gamePhase = GamePhase.GameEnd;

            HideTapTutorial();
            HideRotateTutorial();
            HideZoomTutorial();
            
            EventSystem.instance.TriggerEvent(new LevelFinishEvent(category, level));
            
            SaveManager.instance.UpdateGameProgress(category, currentLevel);
            SaveManager.instance.UpdateSkinProgress();
            
            OpenGameEnd();
        }

        #region Control Function
        
        public void NextLevel()
        {
            if (State.isGameEnd)
            {
                currentLevel++;
            }
        }

        // <-- Start Game -->
        // Note: Start Game will load the map of current level then load the map

        // Note: This function will be called when the game is started
        // Note: Currently in Test so this function is being called in StartGame to continue the game
        //      In the future, this function will be called after Win Game or Lose Game be closed
        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitUntil(() => mapManager.isMapLoaded);
            mapManager.ShowMap(() => { State.gamePhase = GamePhase.Playing; });
            ShowTutorial();
        }

        public void PrepareGame()
        {
            State.gamePhase = GamePhase.Start;
            SetupTutorial();
            mapManager.LoadMap($"Assets/com.ethnicthv/R/Map/{CategoryMap[category]}/{currentLevel}.json");
            //Obsolete: be removed in the future
            StartCoroutine(StartGameCoroutine());
        }
        // <-- end -->

        private void UpdateCurrentLevel()
        {
            PrepareGame();
        }

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

        #endregion

        public void OpenLevelSelector()
        {
            if (State.gamePhase is not (GamePhase.Playing or GamePhase.Start)) return;
            if (GameManager.instance.TryChangeState(ScreenState.LevelSelection, out var main))
            {
                LevelSelectorManager.instance.ShowLevelSelector(main);
            }
        }

        public void OpenSkinSelection()
        {
            if (State.gamePhase is not (GamePhase.Playing or GamePhase.Start)) return;
            Disable();
            if (GameManager.instance.TryChangeState(ScreenState.SkinSelection, out var main))
            {
                SkinSelectionManager.instance.ShowSkinSelection(main);
            }
        }
        
        public void OpenGameEnd()
        {
            if (State.gamePhase is not GamePhase.GameEnd) return;
            Disable();
            if (GameManager.instance.TryChangeState(ScreenState.GameEnd, out var main))
            {
                GameEndManager.instance.ShowGameEnd(main);
            }
        }

        public class GameState
        {
            public bool isPaused => gamePhase == GamePhase.Pause;

            public bool isPlaying => gamePhase == GamePhase.Playing;

            public bool isGameEnd => gamePhase == GamePhase.GameEnd;

            public GamePhase gamePhase { get; protected internal set; }
        }

        public static readonly Dictionary<int, string> CategoryMap = new()
        {
            { 0, "Easy" },
            { 1, "Normal" },
            { 2, "Hard" }
        };

        #region Tutorial

        public void SetupTutorial()
        {
            gameControlTutorialController.Setup(category, currentLevel);
        }

        public void ShowTutorial()
        {
            HideTapTutorial();
            HideRotateTutorial();
            HideZoomTutorial();
            gameControlTutorialController.ShowTutorial();
        }

        public void HideTapTutorial()
        {
            gameControlTutorialController.HideTapTutorial();
        }

        public void HideRotateTutorial()
        {
            gameControlTutorialController.HideRotateTutorial();
        }

        public void HideZoomTutorial()
        {
            gameControlTutorialController.HideMzTutorial();
        }

        public bool IsEnableRotate()
        {
            var config =
                GameInternalSetting.FeatureLockTutorialConfigs[GameInternalSetting.TutorialType.Rotate];
            foreach (var (enableCat, enableLvStart, enableLvEnd) in config)
            {
                if (category == enableCat && currentLevel >= enableLvStart && currentLevel < enableLvEnd)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsEnableZoom()
        {
            var config =
                GameInternalSetting.FeatureLockTutorialConfigs[GameInternalSetting.TutorialType.Move2Zoom];

            foreach (var (enableCat, enableLvStart, enableLvEnd) in config)
            {
                if (category == enableCat && currentLevel >= enableLvStart && currentLevel < enableLvEnd)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }

    public enum GamePhase
    {
        Start,
        Playing,
        Pause,
        GameEnd
    }
}