using System.Collections.Generic;
using System.IO;
using com.ethnicthv.Game.Impl;
using Newtonsoft.Json;
using UnityEngine;

namespace com.ethnicthv.Game.Data
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance { get; private set; }

        private string _saveFilePath;
        private string _gameProgressFilePath;
        private string _skinProgressFilePath;
        private string _playerDataFilePath;

        public GameProgress gameProgressData { get; private set; }
        public SkinProgress skinProgressData { get; private set; }
        public PlayerData playerData { get; private set; }

        private void Awake()
        {
            Debug.Log("SaveManager Awake");
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _saveFilePath = Application.persistentDataPath;
            _gameProgressFilePath = _saveFilePath + "/gameProgress.json";
            _skinProgressFilePath = _saveFilePath + "/skinProgress.json";
            _playerDataFilePath = _saveFilePath + "/playerData.json";
        }

        private void Start()
        {
            Debug.Log("SaveManager Start");
            var firstTime = !(LoadGameProgress() && LoadSkinProgress() && LoadPlayerData());
            if (!firstTime) return;
            CreateNewGameProgress();
            CreateNewSkinProgress();
            CreateNewPlayerData();
        }

        #region Game Progress

        private void UpdateCompleteLevelGroup(int category, int levelGroup)
        {
            var maxLevel = GameInternalSetting.GameLevelFraction[category][levelGroup].Item2;
            gameProgressData.categoryProgress[category][levelGroup] = maxLevel;
        }

        public void UpdateGameProgress(int category, int level)
        {
            var isUnlockCategoryLevel = GameInternalSetting
                .LevelUnlockNewCategory.TryGetValue((category, level), out var newCategory);
            
            var nextLevel = level + 1;
            
            var isLastLevelInGroup = GameInternalSetting
                .IsLastLevelInGroup(category, level, out var levelGroup);
            
            var oldLevelGroup = levelGroup;
            
            UpdateCompleteLevelGroup(category, levelGroup);
            if (GameInternalSetting.GameLevelFraction[category].Count == levelGroup + 1)
            {
                EventSystem.instance.TriggerEvent(new SavedStateLevelFinishEvent(category, level, true, nextLevel,
                    oldLevelGroup, levelGroup,
                    true));
                // Unlock new category
                gameProgressData.UnlockCategory(newCategory);
                EventSystem.instance.TriggerEvent(new UnlockNewCategoryEvent(category, level, newCategory, false));
                return;
            }
            
            if (isLastLevelInGroup)
            {
                levelGroup++;
            }
            
            Debug.Log("isUnlockCategoryLevel: " + isUnlockCategoryLevel + " " + category + " " + level );
            if (isUnlockCategoryLevel)
            {
                Debug.Log("Unlock New Category");
                gameProgressData.UnlockCategory(newCategory);
                EventSystem.instance.TriggerEvent(new UnlockNewCategoryEvent(category, level, newCategory));
            }

            gameProgressData.currentCategory = category;
            gameProgressData.currentLevel = nextLevel;
            if (!gameProgressData.categoryProgress.ContainsKey(category))
            {
                gameProgressData.categoryProgress.Add(category, new Dictionary<int, int>());
            }

            var progress = nextLevel - GameInternalSetting.GameLevelFraction[category][levelGroup].Item1;
            if (!gameProgressData.categoryProgress[category].ContainsKey(levelGroup))
            {
                gameProgressData.categoryProgress[category].Add(levelGroup, 0);
            }

            if (gameProgressData.categoryProgress[category][levelGroup] < progress)
            {
                gameProgressData.categoryProgress[category][levelGroup] = progress;
            }
            
            SaveGameProgress();
            
            EventSystem.instance.TriggerEvent(new SavedStateLevelFinishEvent(category, level, isLastLevelInGroup,
                nextLevel, oldLevelGroup, levelGroup));
        }

        private bool LoadGameProgress()
        {
            if (!File.Exists(_gameProgressFilePath)) return false;
            var json = File.ReadAllText(_gameProgressFilePath);
            gameProgressData = JsonConvert.DeserializeObject<GameProgress>(json);
            return gameProgressData != null;
        }

        private void CreateNewGameProgress()
        {
            gameProgressData = GameProgress.Empty();
            Debug.Log(gameProgressData != null);
            SaveGameProgress();
        }

        public void SaveGameProgress()
        {
            var json = JsonConvert.SerializeObject(gameProgressData);
            File.WriteAllText(_gameProgressFilePath, json);
        }

        #endregion

        #region Skin Progress

        /// <summary>
        /// UpdateSkinProgress is called when the game level is End.
        /// </summary>
        public void UpdateSkinProgress()
        {
            foreach (var (skinId, config) in GameInternalSetting.SkinProgressConfigs)
            {
                SkinProgressUpdater.Update(skinId, config);
            }

            SaveSkinProgress();
        }

        /// <summary>
        /// UpdateGatchaSkin is called when the player get a new skin from Gatcha. (TODO: need to implement Gatcha)
        /// </summary>
        /// <param name="skinId"></param>
        public void UpdateGatchaSkin(int skinId)
        {
            var config = GameInternalSetting.SkinProgressConfigs[skinId];
            if (config.Type == GameInternalSetting.SkinProgressType.Gatcha)
            {
                SkinProgressUpdater.Update(skinId, config);
            }
            else
            {
                Debug.LogError("SkinId is not Gacha Type");
            }
        }

        /// <summary>
        /// UpdatePurchaseSkin is called when the player purchase a new skin. (TODO: need to implement Purchase)
        /// </summary>
        /// <param name="skinId"></param>
        public void UpdatePurchaseSkin(int skinId)
        {
            var config = GameInternalSetting.SkinProgressConfigs[skinId];
            if (config.Type == GameInternalSetting.SkinProgressType.Purchase)
            {
                SkinProgressUpdater.Update(skinId, config);
            }
            else
            {
                Debug.LogError("SkinId is not Purchase Type");
            }
        }

        /// <summary>
        /// UpdateAdsSkin is called when the player watch ads to get a new skin. (TODO: need to implement Ads)
        /// </summary>
        /// <param name="skinId"></param>
        public void UpdateAdsSkin(int skinId)
        {
            var config = GameInternalSetting.SkinProgressConfigs[skinId];
            if (config.Type == GameInternalSetting.SkinProgressType.Ads)
            {
                SkinProgressUpdater.Update(skinId, config);
            }
            else
            {
                Debug.LogError("SkinId is not Ads Type");
            }
        }

        private bool LoadSkinProgress()
        {
            if (!File.Exists(_skinProgressFilePath)) return false;
            var json = File.ReadAllText(_skinProgressFilePath);
            skinProgressData = JsonConvert.DeserializeObject<SkinProgress>(json);
            return skinProgressData != null;
        }

        private void CreateNewSkinProgress()
        {
            skinProgressData = SkinProgress.Empty();
            SaveSkinProgress();
        }

        public void SaveSkinProgress()
        {
            var json = JsonConvert.SerializeObject(skinProgressData);
            File.WriteAllText(_skinProgressFilePath, json);
        }

        #endregion

        #region Player Data
        
        public void RemovePlayerCoins(int amount)
        {
            playerData.RemoveCoins(amount);
            SavePlayerData();
        }
        
        public void AddPlayerCoins(int amount)
        {
            playerData.AddCoins(amount);
            SavePlayerData();
        }

        public void SetPlayerSkin(int skinId)
        {
            playerData.SetCurrentSkin(skinId);
            SavePlayerData();
        }

        private bool LoadPlayerData()
        {
            if (!File.Exists(_playerDataFilePath)) return false;
            var json = File.ReadAllText(_playerDataFilePath);
            playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            return playerData != null;
        }

        private void CreateNewPlayerData()
        {
            playerData = PlayerData.Empty();
            SavePlayerData();
        }

        public void SavePlayerData()
        {
            var json = JsonConvert.SerializeObject(playerData);
            File.WriteAllText(_playerDataFilePath, json);
        }

        #endregion
    }
}