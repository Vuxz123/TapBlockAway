using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.ethnicthv.Game.LevelSelection
{
    public class LevelGroupController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private GameObject content;
        [SerializeField] private Image levelBorder;
        [SerializeField] private Image levelViewBtn;
        [SerializeField] private Image levelIcon;
        [SerializeField] private TextMeshProUGUI levelTitle;
        [SerializeField] private Image levelProgress;
        [SerializeField] private TextMeshProUGUI levelProgressText;
        
        private CategoryController _categoryController;
        private Action _onView = () => { };
        
        public void SetupLevelGroup(
            CategoryController categoryController,
            bool locked,
            int startLevel, 
            int maxLevel, 
            int levels, 
            Action onView)
        {
            _categoryController = categoryController;
            _onView = onView;
            
            levelTitle.text = $"Level {startLevel} - {startLevel + maxLevel - 1}";
            levelProgress.fillAmount = levels / (float) maxLevel;
            levelProgressText.text = $"{levels}/{maxLevel}";

            if (locked)
            {
                levelBorder.color = _categoryController.levelGroupLockedColor;
                levelViewBtn.color = _categoryController.levelGroupLockedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[2];
                levelIcon.color = _categoryController.levelGroupLockedColor;
                return;
            }
            
            if (maxLevel == levels)
            {
                Debug.Log("Level Group Completed");
                levelBorder.color = _categoryController.levelGroupCompletedColor;
                levelViewBtn.color = _categoryController.levelGroupCompletedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[0];
            }
            else
            {
                Debug.Log("Level Group Not Completed");
                levelBorder.color = _categoryController.levelGroupNotCompletedColor;
                levelViewBtn.color = _categoryController.levelGroupNotCompletedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[1];
                levelIcon.color = _categoryController.levelGroupLockedColor;
            }
        }

        public void Hide(float levelGroupOffset)
        {
            
            content.transform.DOLocalMoveX(- levelGroupOffset, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                content.SetActive(false);
            });
        }

        public void Show(float levelGroupOffset)
        {
            content.transform.localPosition = new Vector2(
                levelGroupOffset, 
                content.transform.localPosition.y);
            content.SetActive(true);
            content.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.InOutSine);
        }

        public void View()
        {
            Debug.Log("View Level Group");
            _onView();
        }
    }
}