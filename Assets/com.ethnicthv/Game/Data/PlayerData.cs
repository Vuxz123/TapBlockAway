using System;
using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class PlayerData
    {
        public int currentSkin;
        public List<int> badges;
        
        public static PlayerData Empty()
        {
            return new PlayerData
            {
                currentSkin = 0
            };
        }
        
        public void SetCurrentSkin(int skinId)
        {
            currentSkin = skinId;
        }
        
        public void AddBadge(int badgeId)
        {
            if (badges.Contains(badgeId)) return;
            badges.Add(badgeId);
        }
        
        public bool HasBadge(int badgeId)
        {
            return badges.Contains(badgeId);
        }
        
        
    }
}