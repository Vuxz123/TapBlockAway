using System;

namespace com.ethnicthv.Game.Map
{
    [Serializable]
    public class Map
    {
        public int size;

        public bool shiftX;
        public bool shiftY;
        public bool shiftZ;
        
        public int[] map;
    }
}