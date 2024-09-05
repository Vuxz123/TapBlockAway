using System.Linq;
using com.ethnicthv.Game.Cube;
using UnityEngine;

namespace com.ethnicthv.Game.Home
{
    [CreateAssetMenu(fileName = "Cube Skin Database", menuName = "Database", order = 0)]
    public class CubeSkinDatabase : ScriptableObject
    {
        public Skin[] skins;
        
        public int length => skins.Length;
        
        public Skin GetSkin(string skinName)
        {
            return skins.FirstOrDefault(skin => skin.name == skinName);
        }
        
        public Skin GetSkin(int index)
        {
            return skins[index];
        }
    }
}