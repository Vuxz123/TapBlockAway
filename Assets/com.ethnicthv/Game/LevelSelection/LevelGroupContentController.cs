using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelGroupContentController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private List<LevelItemController> levelControllers;
        
        public void SetupLevelGroupContent(int categoryIndex, int itemGroupIndex)
        {
            for (int i = 0; i < levelControllers.Count; i++)
            {
                var unlocked = false; //TODO: Implement logic
                var completed = false; //TODO: Implement logic
                levelControllers[i].SetupLevelItem(i, unlocked, completed);
            }
        }

        public void Show()
        {
            transform.localPosition = new Vector3(
                1920,
                transform.localPosition.y,
                0);
            gameObject.SetActive(true);
            transform.DOLocalMoveX(0, 0.5f);
        }

        public void Hide()
        {
            transform.DOLocalMoveX(1920, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}