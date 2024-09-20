using System;
using System.Collections.Generic;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class PlayerData
    {
        public int coins;
        public int currentSkin;
        public List<int> badges;
        
        public static PlayerData Empty()
        {
            return new PlayerData
            {
                coins = 0,
                currentSkin = 0,
                badges = new List<int>()
            };
        }
        
        public bool IsCurrentSkin(int skinId)
        {
            return currentSkin == skinId;
        }
        
        public void SetCurrentSkin(int skinId)
        {
            currentSkin = skinId;
            EventSystem.instance.TriggerEvent(new PlayerSkinChangeEvent(skinId));
        }
        
        public void AddBadge(int badgeId)
        {
            if (badges.Contains(badgeId)) return;
            badges.Add(badgeId);
            EventSystem.instance.TriggerEvent(new PlayerBadgeChangeEvent(badges));
        }
        
        public bool HasBadge(int badgeId)
        {
            return badges.Contains(badgeId);
        }
        
        public void AddCoins(int amount)
        {
            coins += amount;
            if (coins < 0) throw new ArgumentOutOfRangeException(nameof(coins));
            EventSystem.instance.TriggerEvent(new PlayerCoinChangeEvent(coins));
        }
        
        public void RemoveCoins(int amount)
        {
            coins -= amount;
            if (coins < 0) throw new ArgumentOutOfRangeException(nameof(coins));
            EventSystem.instance.TriggerEvent(new PlayerCoinChangeEvent(coins));
        }
        
        public bool HasEnoughCoins(int amount)
        {
            return coins >= amount;
        }
    }
}