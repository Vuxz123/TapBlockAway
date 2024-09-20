namespace com.ethnicthv.Game
{
    public class PlayerCoinChangeEvent : Event
    {
        public readonly int CoinCount;
        
        public PlayerCoinChangeEvent(int coinCount) : base(20)
        {
            CoinCount = coinCount;
        }
    }
}