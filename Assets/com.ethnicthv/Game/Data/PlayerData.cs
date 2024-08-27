using System;

namespace com.ethnicthv.Game.Data
{
    [Serializable]
    public class PlayerData
    {
        public int currentSkin;
        
        public static PlayerData Empty()
        {
            return new PlayerData
            {
                currentSkin = 0
            };
        }
    }
}