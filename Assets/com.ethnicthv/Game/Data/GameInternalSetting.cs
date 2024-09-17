using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.InputSystem.Utilities;

namespace com.ethnicthv.Game.Data
{
    public static class GameInternalSetting
    {
        #region Game Level Fracturization

        // Note: <category, <levelGroup, (levelStart, levelMax)>>
        public static readonly List<List<(int, int)>> GameLevelFraction = new()
        {
            new List<(int, int)>() // Easy
            {
                (0, 10),
                (10, 50),
                (50, 400)
            },
            new List<(int, int)>(), // Normal
            new List<(int, int)>() // Hard
        };
        
        public static (int, int) FindLevelGroup(int category, int level)
        {
            var levelGroup = 0;
            var levelInGroup = 0;
            var fracture = GameLevelFraction[category];
            for (var i = 0; i < fracture.Count; i++)
            {
                if (level < fracture[i].Item1 || level >= fracture[i].Item2) continue;
                levelGroup = i;
                levelInGroup = level - fracture[i].Item1;
                break;
            }
            return (levelGroup, levelInGroup);
        }

        public static bool IsLastLevelInGroup(int category, int level, out int levelGroup)
        {
            var fracture = GameLevelFraction[category];
            for (var i = 0; i < fracture.Count; i++)
            {
                if (level < fracture[i].Item1 || level >= fracture[i].Item2) continue;
                levelGroup = i;
                return level == fracture[i].Item2 - 1;
            }
            levelGroup = 0;
            return false;
        }

        #endregion

        #region Game Skin Progress Config

        public enum SkinProgressType
        {
            Ads,
            PlayProgress,
            Purchase,
            Gatcha,
            Gift
        }
        
        public class SkinProgressConfig
        {
            public readonly SkinProgressType Type;
            public readonly int LevelUnlock;
            public readonly int PurchasePrice;
            public readonly Func<bool> Condition;

            public SkinProgressConfig(SkinProgressType type, int levelUnlock = 0, int purchasePrice = 100, Func<bool> condition = null)
            {
                Type = type;
                LevelUnlock = levelUnlock;
                PurchasePrice = purchasePrice;
                Condition = condition;
            }
        }
        
        public static readonly Dictionary<int, SkinProgressConfig> SkinProgressConfigs = new()
        {
            // {0, new SkinProgressConfig(SkinProgressType.Ads, 0, 0, () => true)},
            // {1, new SkinProgressConfig(SkinProgressType.PlayProgress, 10, 0, () => true)},
            // {2, new SkinProgressConfig(SkinProgressType.Purchase, 0, 1000, () => true)},
            // {3, new SkinProgressConfig(SkinProgressType.Gatcha, 0, 0, () => true)},
            // {4, new SkinProgressConfig(SkinProgressType.Gift, 0, 0, () => SaveManager.instance.playerData.HasBadge(0))}
            {2, new SkinProgressConfig(SkinProgressType.PlayProgress, 10)},
            {3, new SkinProgressConfig(SkinProgressType.PlayProgress, 20)},
            {4, new SkinProgressConfig(SkinProgressType.PlayProgress, 40)}
        };
        
        public static ReadOnlyCollection<int> GetSkinOfType(SkinProgressType type)
        {
            var skins = new List<int>();
            foreach (var (skinId, config) in SkinProgressConfigs)
            {
                if (config.Type == type)
                {
                    skins.Add(skinId);
                }
            }
            return skins.AsReadOnly();
        }

        #endregion

        #region Game Tutorial Config

        public enum TutorialType
        {
            None,
            Tap,
            Rotate,
            Move2Zoom
        }
        
        // Note: <tutorialType, (category, level)>
        public static readonly Dictionary<(int, int),TutorialType> TutorialConfigs = new()
        {
            {(0, 0), TutorialType.Tap},
            {(0, 1), TutorialType.Rotate},
            {(0, 2), TutorialType.Move2Zoom},
        };
        
        // Note: <tutorialType, (category, levelLockStart, levelLockEnd)>
        public static readonly Dictionary<TutorialType, (int, int, int)[]> FeatureLockTutorialConfigs = new()
        {
            {TutorialType.Rotate, new[] {(0, 0, 1)}},
            {TutorialType.Move2Zoom, new[] {(0, 0, 2)}}
        };
        
        public static TutorialType GetTutorialType(int category, int level)
        {
            return TutorialConfigs.GetValueOrDefault((category, level), TutorialType.None);
        }

        #endregion

        #region Game Category Config
        
        // Note: <(category, level), new category>
        public static readonly Dictionary<(int, int), int> LevelUnlockNewCategory = new()
        {
            {(0, 10), 1},
            {(1, 10), 2}
        };

        #endregion

        #region Challenge Config
        
        // Note: <(category, level), challenge level>
        public static readonly Dictionary<(int, int), int> ChallengeLevel = new()
        {
            {(0,1), 0}
        };

        #endregion
    }
}