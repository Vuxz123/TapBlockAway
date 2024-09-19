using System;
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

        private void OnEnable()
        {
            RunAnimation();
        }

        public void UpdateProgress(int newProgress)
        {
            _currentProgressInterval = 0;
            progress = newProgress;
        }
        
        public void RunAnimation()
        {
            if (progress == total)
            {
                progress++;
            }
            DOTween.To(() => _currentProgressInterval, x => _currentProgressInterval = x, progress, 5f)
                .OnUpdate(OnUpdateInterval);
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