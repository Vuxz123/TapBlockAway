using System;
using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class GameProgress
    {
        public int currentLevel;
        public int currentCategory;
        public Dictionary<int, Dictionary<int, int>> categoryProgress;
        
        public static GameProgress Empty()
        {
            var n = new GameProgress
            {
                currentLevel = 0,
                currentCategory = 0,
                categoryProgress = new Dictionary<int, Dictionary<int, int>>()
            };
            var lc = new Dictionary<int, int>()
            {
                { 0, 0 }
            };
            n.categoryProgress.Add(0, lc);
            return n;
        }
    }
}