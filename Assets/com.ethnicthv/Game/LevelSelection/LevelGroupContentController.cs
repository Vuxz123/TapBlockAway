using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelGroupContentController : MonoBehaviour, IPopable
    {
        [Header("Setup")]
        [SerializeField] private List<LevelItemController> levelControllers;
        [SerializeField] private Transform levelItemParent;
        
        private bool _isShowed;
        
        public void SetupLevelGroupContent(CategoryController controller, int startLevel, int maxLevel, int levels)
        {
            for (int i = 0; i < maxLevel; i++)
            {
                var currentLevel = startLevel + i;
                var unlocked = i <= levels;
                var completed = i != levels;
                if (levelControllers.Count <= i)
                {
                    levelControllers.Add(Instantiate(levelControllers[0], levelItemParent));
                }
                levelControllers[i].SetupLevelItem(controller, currentLevel, unlocked, completed);
            }
            for (int i = maxLevel; i < levelControllers.Count; i++)
            {
                levelControllers[i].gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            _isShowed = true;
            LevelSelectorManager.instance.PushPopable(this);
            transform.localPosition = new Vector3(
                1920,
                transform.localPosition.y,
                0);
            gameObject.SetActive(true);
            transform.DOLocalMoveX(0, 0.5f);
        }

        public void Hide()
        {
            if (!_isShowed) return;
            _isShowed = false;
            transform.DOLocalMoveX(1920, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        public void Pop()
        {
            Hide();
        }
    }
}