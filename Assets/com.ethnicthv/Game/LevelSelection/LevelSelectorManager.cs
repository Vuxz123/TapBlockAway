using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using com.ethnicthv.Game.Gameplay;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelSelectorManager : MonoBehaviour
    {
        public static LevelSelectorManager instance { get; private set; }

        private Queue<IPopable> _popables = new();

        [SerializeField] private CategoryController categoryController;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Transform movable;

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
                popable.Pop();
                return true;
            }

            popable = null;
            return false;
        }

        #endregion

        public void Back()
        {
            Debug.Log("Back");
            if (HasPopable())
            {
                if (!TryPop(out var pop))
                {
                    Debug.LogError($"Cannot pop from queue.");
                }
            }
            else
            {
                HideLevelSelector();
            }
        }

        public void ForceBack()
        {
            PopAll();
            HideLevelSelector();
        }

        private void HideLevelSelector()
        {
            if (!GameManager.instance.TryChangeState(ScreenState.GamePlay, out var main)) return;
            main.Enable();
            movable.DOLocalMoveX(2000, 0.5f).OnComplete(() =>
            {
                categoryController.Reset();
                canvas.SetActive(false);
            });
        }
        
        public void ShowLevelSelector(GamePlayManager main)
        {
            movable.localPosition = new Vector3(
                2000,
                transform.localPosition.y,
                0);
            canvas.SetActive(true);
            movable.DOLocalMoveX(0, 0.5f).OnComplete(() =>
            {
                main.Disable();
                categoryController.OpenCurrentCategory();
            });
        }
        
        public void OpenLevel(int category, int level)
        {
            ForceBack();
            GamePlayManager.instance.category = category;
            GamePlayManager.instance.currentLevel = level;
        }
    }
}