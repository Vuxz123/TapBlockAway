using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace com.ethnicthv.Game.LevelSelection
{
    public class CategoryController : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private Image categoryBar;
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

        [Header("Level Group Items")] 
        [SerializeField] private float levelGroupOffset;
        [SerializeField] private List<LevelGroupController> firstLevelGroups;
        [SerializeField] private List<LevelGroupController> secondLevelGroups;
        [SerializeField] private Image firstLevelGroupsTarget;
        [SerializeField] private Image secondLevelGroupsTarget;
        private bool _isFirstLevelGroupShowed;
        private bool _isSwapping;
        
        [Header("Level Group Content")]
        [SerializeField] private LevelGroupContentController levelGroupContentDisplay;

        private void Start()
        {
            DOVirtual.DelayedCall(2f, SelectEasy);
        }

        public void SelectEasy()
        {
            if (_isSwapping) return;
            Debug.Log("Easy");
            easyLayoutElement.padding.top = 0;
            normalLayoutElement.padding.top = 20;
            hardLayoutElement.padding.top = 20;

            categoryBar.sprite = categoryBarSpites[0];

            OpenCategory(0);
            
            MarkForRebuild();
        }

        public void SelectNormal()
        {
            if (_isSwapping) return;
            Debug.Log("Normal");
            easyLayoutElement.padding.top = 20;
            normalLayoutElement.padding.top = 0;
            hardLayoutElement.padding.top = 20;

            categoryBar.sprite = categoryBarSpites[1];
            
            OpenCategory(1);

            MarkForRebuild();
        }

        public void SelectHard()
        {
            if (_isSwapping) return;
            Debug.Log("Hard");
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
        
        private void OpenLevelGroup(int category, int levelGroupIndex)
        {
            levelGroupContentDisplay.SetupLevelGroupContent(category, levelGroupIndex);
            levelGroupContentDisplay.Show();
        }
        
        #endregion

        #region Level Group Display

        private void OpenCategory(int categoryIndex)
        {
            _isSwapping = true; // Note: set the swapping state to true
            var levelGroups = GetLevelGroupList();

            // TODO: Get the easy category levels list from file
            for (var i = 0; i < levelGroups.Count; i++)
            {
                var start = Random.Range(0, 10);
                var maxLevel = Random.Range(0, 20);
                var levels = Random.Range(0, maxLevel);
                levelGroups[i].SetupLevelGroup(
                    this, Random.Range(0, 2) != 0, 
                    start, maxLevel, levels, () =>
                    {
                        OpenLevelGroup(categoryIndex, 1);
                    });
            }

            StartCoroutine(SwapLevelGroupDisplay());
        }

        private IEnumerator SwapLevelGroupDisplay()
        {
            var (hideList, showList) = _isFirstLevelGroupShowed ? 
                (firstLevelGroups, secondLevelGroups): 
                (secondLevelGroups, firstLevelGroups);
            
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