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
        
        private bool _isEmpty = true;
        
        public void SetupLevelGroup(
            CategoryController categoryController,
            bool locked,
            int startLevel, 
            int maxLevel, 
            int levels, 
            Action onView)
        {
            _isEmpty = false;
            
            _categoryController = categoryController;
            _onView = onView;
            
            levelTitle.text = $"Level {startLevel + 1} - {startLevel + maxLevel}";

            if (locked)
            {
                levelBorder.color = _categoryController.levelGroupLockedColor;
                levelViewBtn.color = _categoryController.levelGroupLockedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[2];
                levelIcon.color = _categoryController.levelGroupLockedColor;
                
                levelProgress.fillAmount = 0;
                levelProgressText.text = $"0/{maxLevel}";
                return;
            }
            
            if (levels >= maxLevel)
            {
                Debug.Log("Level Group Completed");
                levelBorder.color = _categoryController.levelGroupCompletedColor;
                levelViewBtn.color = _categoryController.levelGroupCompletedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[0];
                levelIcon.color = Color.white;
                
                levelProgress.fillAmount = 1;
                levelProgressText.text = $"{maxLevel}/{maxLevel}";
            }
            else
            {
                Debug.Log("Level Group Not Completed");
                levelBorder.color = _categoryController.levelGroupNotCompletedColor;
                levelViewBtn.color = _categoryController.levelGroupNotCompletedColor;
                levelIcon.sprite = _categoryController.levelGroupSprites[1];
                levelIcon.color = _categoryController.levelGroupLockedColor;
                
                levelProgress.fillAmount = (levels + 1) / (float) maxLevel;
                levelProgressText.text = $"{levels + 1}/{maxLevel}";
            }
        }
        
        public void Empty()
        {
            _isEmpty = true;
            content.SetActive(false);
        }

        public void Hide(float levelGroupOffset)
        {
            if (_isEmpty) return;
            content.transform.DOLocalMoveX(- levelGroupOffset, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                content.SetActive(false);
            });
        }

        public void Show(float levelGroupOffset)
        {
            if (_isEmpty) return;
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

        public void Reset()
        {
            Empty();
        }
    }
}