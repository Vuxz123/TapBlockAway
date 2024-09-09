using System;
using UnityEngine;

namespace com.ethnicthv.Game
{
    public class DisableAble : MonoBehaviour
    {
        public event Action onDisable;
        public event Action onEnable;
        
        private void OnEnable()
        {
            onEnable?.Invoke();
        }
        
        private void OnDisable()
        {
            onDisable?.Invoke();
        }
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}