namespace com.ethnicthv.Game
{
    public class PlayerSkinChangeEvent : Event
    {
        public readonly int SkinId;
        
        public PlayerSkinChangeEvent(int skinId) : base(30)
        {
            SkinId = skinId;
        }
    }
}