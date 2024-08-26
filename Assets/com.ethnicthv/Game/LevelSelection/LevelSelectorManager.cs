using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelSelectorManager : MonoBehaviour
    {
        public static LevelSelectorManager instance { get; private set; }
        
        private Queue<IPopable> _popables = new();

        private void Awake()
        {
            instance = this;
        }

        #region Popable

        public void PushPopable(IPopable popable)
        {
            _popables.Enqueue(popable);
        }
        
        public void PopAll()
        {
            while (_popables.Count > 0)
            {
                _popables.Dequeue().Pop();
            }
        }
        
        public bool HasPopable()
        {
            return _popables.Count > 0;
        }
        
        public bool TryPop(out IPopable popable)
        {
            if (_popables.Count > 0)
            {
                popable = _popables.Dequeue();
                return true;
            }

            popable = null;
            return false;
        }

        #endregion
    }
}