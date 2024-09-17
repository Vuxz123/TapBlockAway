using System;
using System.Collections.Generic;
using System.Linq;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class GameProgress
    {
        public int currentLevel;
        public int currentCategory;
        // Note: <category, <levelGroup, lastUnlockedLevel>>
        public Dictionary<int, Dictionary<int, int>> categoryProgress;
        
        public void UnlockCategory(int category)
        {
            if (categoryProgress.ContainsKey(category)) return;
            categoryProgress.Add(category, new Dictionary<int, int>{{0,0}});
        }
        
        public int GetNumberOfCompletedLevels()
        {
            var count = categoryProgress
                .SelectMany(category => category.Value)
                .Sum(levelGroup => levelGroup.Value);
            return count;
        }

        public static GameProgress Empty()
        {
            var n = new GameProgress
            {
                currentLevel = 0,
                currentCategory = 0,
                categoryProgress = new Dictionary<int, Dictionary<int, int>>
                {
                    {
                        0, new Dictionary<int, int>
                        {
                            { 0, 0 }
                        }
                    }
                }
            };
            return n;
        }
    }
}