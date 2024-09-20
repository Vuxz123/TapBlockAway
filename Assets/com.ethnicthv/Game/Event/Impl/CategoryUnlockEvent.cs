namespace com.ethnicthv.Game
{
    public class CategoryUnlockEvent : Event
    {
        public readonly int Category;
        
        public CategoryUnlockEvent(int category) : base(4)
        {
            Category = category;
        }
    }
}