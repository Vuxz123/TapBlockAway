using System;
using com.ethnicthv.Game.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace com.ethnicthv.Game.GameEnd
{
    public class CoinDisplayController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        
        private int _playerCoinChangeListenerId;

        private void OnEnable()
        {
            coinText.text = SaveManager.instance.playerData.coins.ToString();
            _playerCoinChangeListenerId = EventSystem.instance.RegisterListener<PlayerCoinChangeEvent>(OnPlayerCoinChange);
        }
        
        private void OnDisable()
        {
            EventSystem.instance.UnregisterListener<PlayerCoinChangeEvent>(_playerCoinChangeListenerId);
        }

        private void OnPlayerCoinChange(PlayerCoinChangeEvent playerCoinChangeEvent)
        {
            Debug.Log("Player coin changed: " + playerCoinChangeEvent.CoinCount);
            coinText.text = playerCoinChangeEvent.CoinCount.ToString();
            transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f, 1, 0.5f);
        }
    }
}