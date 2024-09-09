using System.Linq;
using UnityEngine;

namespace com.ethnicthv.Game.Cube.CubeSkin
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