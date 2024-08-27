using System;
using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class SkinProgress
    {
        public Dictionary<int, bool> skinUnlocked;
        public Dictionary<int, float> skinProgressing;

        public static SkinProgress Empty()
        {
            return new SkinProgress
            {
                skinUnlocked = new Dictionary<int, bool>(),
                skinProgressing = new Dictionary<int, float>()
            };
        }
    }
}