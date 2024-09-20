using com.ethnicthv.Game.Data;
using com.ethnicthv.Game.Gameplay;
using com.ethnicthv.Game.Impl;
using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class GameEndManager : MonoBehaviour
    {
        public static GameEndManager instance { get; private set; }
        
        [SerializeField] private DisableAble disableAble;
        [SerializeField] private CoinProgressController coinProgressController;
        
        private int _levelFinishListenerId;
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            disableAble.onDisable += () =>
            {
                coinProgressController.Reset();
            };
            _levelFinishListenerId = EventSystem.instance.RegisterListener<LevelFinishEvent>(OnLevelFinished);
        }
        
        private void OnLevelFinished(LevelFinishEvent levelFinishEvent)
        {
            var p = SaveManager.instance.gameProgressData.GetNumberOfCompletedLevels() % 5 + 1;
            Debug.Log("Progress: " + p);
            coinProgressController.UpdateProgress(p);
        }
        
        public void ShowGameEnd(GamePlayManager gamePlayManager)
        {
            disableAble.Enable();
        }
        
        public void CloseGameEndThenNextLevel()
        {
            if (GameManager.instance.TryChangeState(ScreenState.GamePlay, out var main))
            {
                disableAble.Disable();
                main.Enable();
                main.NextLevel();
            }
        }
    }
}