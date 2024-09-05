using System.Collections.Generic;
using System.IO;
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

        public void UpdateCompleteLevelGroup(int category, int levelGroup)
        {
            var maxLevel = GameInternalSetting.GameLevelFraction[category][levelGroup].Item2;
            gameProgressData.categoryProgress[category][levelGroup] = maxLevel;
        }

        public void UpdateGameProgress(int category, int levelGroup, int level)
        {
            level += 1;
            gameProgressData.currentCategory = category;
            gameProgressData.currentLevel = level;
            if (!gameProgressData.categoryProgress.ContainsKey(category))
            {
                gameProgressData.categoryProgress.Add(category, new Dictionary<int, int>());
            }
            var progress = level - GameInternalSetting.GameLevelFraction[category][levelGroup].Item1;
            if (!gameProgressData.categoryProgress[category].ContainsKey(levelGroup))
            {
                gameProgressData.categoryProgress[category].Add(levelGroup, 0);
            }
            if (gameProgressData.categoryProgress[category][levelGroup] < progress)
            {
                gameProgressData.categoryProgress[category][levelGroup] = progress;
            }
        }
        
        public void UpdateSkinProgress(int skinId, float progress)
        {
            if (Mathf.Approximately(progress, 1))
            {
                skinProgressData.UnlockSkin(skinId);
            }
            skinProgressData.SetSkinProgress(skinId, progress);
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