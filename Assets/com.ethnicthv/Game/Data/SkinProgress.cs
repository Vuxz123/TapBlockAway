using System;
using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class SkinProgress
    {
        public Dictionary<int, bool> skinUnlocked;
        public Dictionary<int, float> skinProgressing;

        public bool IsSkinUnlocked(int skinId)
        {
            return skinUnlocked.ContainsKey(skinId) && skinUnlocked[skinId];
        }
        
        public float GetSkinProgress(int skinId)
        {
            return skinProgressing.GetValueOrDefault(skinId, 0);
        }
        
        public void UnlockSkin(int skinId)
        {
            skinUnlocked[skinId] = true;
        }
        
        public void SetSkinProgress(int skinId, float progress)
        {
            skinProgressing[skinId] = progress;
        }
        
        public static SkinProgress Empty()
        {
            return new SkinProgress
            {
                skinUnlocked = new Dictionary<int, bool>
                {
                    {0, true}
                },
                skinProgressing = new Dictionary<int, float>()
            };
        }
    }
}