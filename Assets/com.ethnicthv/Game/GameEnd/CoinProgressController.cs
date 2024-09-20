using System;
using com.ethnicthv.Game.Data;
using com.ethnicthv.Game.Impl;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace com.ethnicthv.Game.GameEnd
{
    public class CoinProgressController : MonoBehaviour
    {
        [SerializeField] private int progress = 0;
        [SerializeField] private int total = 5;
        [SerializeField] private Image progressFill;
        [SerializeField] private WaypointController[] waypoints;
        private float _currentProgressInterval = 0;
        
        private int _levelFinishListenerId;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            EventSystem.instance.UnregisterListener<LevelFinishEvent>(_levelFinishListenerId);
        }

        public void Reset()
        {
            foreach (var waypoint in waypoints)
            {
                waypoint.SetHighlight(false);
                waypoint.SetActive(false);
            }
        }

        public void UpdateProgress(int newProgress)
        {
            _currentProgressInterval = 0;
            progress = newProgress;
        }
        
        public void RunAnimation()
        {
            var tier = progress;
            var time = progress;
            if (progress == total)
            {
                progress++;
            }
            DOTween.To(() => _currentProgressInterval, x => _currentProgressInterval = x, progress, time)
                .OnUpdate(OnUpdateInterval).OnComplete(() =>
                {
                    SaveManager.instance.AddPlayerCoins(GameInternalSetting.CoinProgress[tier-1]);
                });
        }
        
        private void OnUpdateInterval()
        {
            progressFill.fillAmount = _currentProgressInterval / (total + 1);
            waypoints[(int)_currentProgressInterval-1].SetActive(true);
            if (Mathf.Approximately(_currentProgressInterval, progress))
            {
                waypoints[progress-1].SetHighlight(true);
            }
        }
    }
}