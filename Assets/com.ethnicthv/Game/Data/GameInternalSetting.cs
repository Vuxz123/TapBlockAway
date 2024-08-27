using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    public class GameInternalSetting
    {
        // Note: <category, <levelGroup, (levelStart, levelMax)>>
        public static List<List<(int, int)>> GameLevelFraction = new List<List<(int, int)>>()
        {
            new() // Easy
            {
                (0, 10),
                (10, 50),
                (50, 400)
            },
            new(), // Normal
            new() // Hard
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
    }
}