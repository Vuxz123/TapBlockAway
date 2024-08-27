using System;
using System.Collections;
using System.Collections.Generic;
using com.ethnicthv.Game.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace com.ethnicthv.Game.LevelSelection
{
    public class CategoryController : MonoBehaviour
    {
        [Header("Setup")] [SerializeField] private Image categoryBar;
        [SerializeField] private HorizontalLayoutGroup easyLayoutElement;
        [SerializeField] private HorizontalLayoutGroup normalLayoutElement;
        [SerializeField] private HorizontalLayoutGroup hardLayoutElement;
        [SerializeField] private List<Sprite> categoryBarSpites;
        [SerializeField] private List<GameObject> categoryList;

        // Note: level group shared assets
        public List<Sprite> levelGroupSprites;
        public Color levelGroupCompletedColor;
        public Color levelGroupNotCompletedColor;
        public Color levelGroupLockedColor;

        [Header("Level Group Items")] [SerializeField]
        private float levelGroupOffset;

        [SerializeField] private List<LevelGroupController> firstLevelGroups;
        [SerializeField] private List<LevelGroupController> secondLevelGroups;
        [SerializeField] private Image firstLevelGroupsTarget;
        [SerializeField] private Image secondLevelGroupsTarget;

        private bool _isFirstLevelGroupShowed;
        private bool _isSwapping;

        [Header("Level Group Content")] [SerializeField]
        private LevelGroupContentController levelGroupContentDisplay;

        private int _categoryIndex = -1;
        
        public int categoryIndex => _categoryIndex;

        public void Reset()
        {
            _categoryIndex = -1;
            _isFirstLevelGroupShowed = false;
            _isSwapping = false;

            foreach (var l in firstLevelGroups)
            {
                l.Reset();
            }

            foreach (var l in secondLevelGroups)
            {
                l.Reset();
            }
        }

        public void OpenCurrentCategory()
        {
            switch (SaveManager.instance.gameProgressData.currentCategory)
            {
                case 0:
                    SelectEasy();
                    break;
                case 1:
                    SelectNormal();
                    break;
                case 2:
                    SelectHard();
                    break;
            }
        }

        public void SelectEasy()
        {
            if (_categoryIndex == 0) return;
            if (_isSwapping) return;

            levelGroupContentDisplay.Hide();

            Debug.Log("Easy");

            _categoryIndex = 0;

            easyLayoutElement.padding.top = 0;
            normalLayoutElement.padding.top = 20;
            hardLayoutElement.padding.top = 20;

            categoryBar.sprite = categoryBarSpites[0];

            OpenCategory(0);

            MarkForRebuild();
        }

        public void SelectNormal()
        {
            if (_categoryIndex == 1) return;
            if (_isSwapping) return;

            levelGroupContentDisplay.Hide();

            Debug.Log("Normal");

            _categoryIndex = 1;

            easyLayoutElement.padding.top = 20;
            normalLayoutElement.padding.top = 0;
            hardLayoutElement.padding.top = 20;

            categoryBar.sprite = categoryBarSpites[1];

            OpenCategory(1);

            MarkForRebuild();
        }

        public void SelectHard()
        {
            if (_categoryIndex == 2) return;
            if (_isSwapping) return;

            levelGroupContentDisplay.Hide();

            Debug.Log("Hard");

            _categoryIndex = 2;

            easyLayoutElement.padding.top = 20;
            normalLayoutElement.padding.top = 20;
            hardLayoutElement.padding.top = 0;

            categoryBar.sprite = categoryBarSpites[2];

            OpenCategory(2);

            MarkForRebuild();
        }

        private void MarkForRebuild()
        {
            LayoutRebuilder.MarkLayoutForRebuild(easyLayoutElement.transform as RectTransform);
            LayoutRebuilder.MarkLayoutForRebuild(normalLayoutElement.transform as RectTransform);
            LayoutRebuilder.MarkLayoutForRebuild(hardLayoutElement.transform as RectTransform);
        }

        #region Level Group Content

        private void OpenLevelGroup(int startLevel, int maxLevel, int levels)
        {
            levelGroupContentDisplay.SetupLevelGroupContent(this, startLevel, maxLevel, levels);
            levelGroupContentDisplay.Show();
        }

        #endregion

        #region Level Group Display

        private void OpenCategory(int catId)
        {
            _isSwapping = true; // Note: set the swapping state to true
            var levelGroups = GetLevelGroupList();

            // Note: get static level fraction data
            var levelGroupData = GameInternalSetting.GameLevelFraction[catId];
            
            // Note: get the category progress data
            var hasCatProgress = SaveManager.instance.gameProgressData.categoryProgress
                .TryGetValue(catId, out var categoryProgress);
            
            var c = levelGroupData.Count; // Note: get the number of level group
            Debug.Log("Num of Level Group: " + levelGroupData.Count);
            for (var i = 0; i < levelGroups.Count; i++)
            {
                if (i < c)
                {
                    // Note: get static level group data
                    var start = levelGroupData[i].Item1;
                    var maxLevel = levelGroupData[i].Item2;
                    
                    // Note: get level group progress data
                    var level = 0;
                    var hasLevelProgress = false;
                    if (hasCatProgress)
                    {
                        hasLevelProgress = categoryProgress.TryGetValue(i, out var levelProgress);
                        if (hasLevelProgress)
                        {
                            level = levelProgress;
                        }
                    }
                    
                    Debug.Log("Level Group: " + i + " Start: " + start + " Max: " + maxLevel + " Level: " + level);
                    levelGroups[i].SetupLevelGroup(
                        this, !hasLevelProgress,
                        start, maxLevel, level, () => { OpenLevelGroup(start, maxLevel, level); });
                }
                else
                {
                    levelGroups[i].Empty();
                }
            }

            StartCoroutine(SwapLevelGroupDisplay());
        }

        private IEnumerator SwapLevelGroupDisplay()
        {
            var (hideList, showList) = _isFirstLevelGroupShowed
                ? (firstLevelGroups, secondLevelGroups)
                : (secondLevelGroups, firstLevelGroups);

            if (_isFirstLevelGroupShowed)
            {
                firstLevelGroupsTarget.raycastTarget = false;
                secondLevelGroupsTarget.raycastTarget = true;
            }
            else
            {
                firstLevelGroupsTarget.raycastTarget = true;
                secondLevelGroupsTarget.raycastTarget = false;
            }

            _isFirstLevelGroupShowed = !_isFirstLevelGroupShowed;
            var c = hideList.Count;

            for (var i = 0; i < c; i++)
            {
                hideList[i].Hide(levelGroupOffset);
                showList[i].Show(levelGroupOffset);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.4f);
            _isSwapping = false; // Note: set the swapping state to false
        }

        private List<LevelGroupController> GetLevelGroupList()
        {
            return _isFirstLevelGroupShowed ? secondLevelGroups : firstLevelGroups;
        }

        #endregion
    }
}