namespace com.ethnicthv.Game.Impl
{
    /// <summary>
    /// This event is used to notify that the level has been finished.
    /// </summary>
    public class LevelFinishEvent : Event
    {
        public readonly int Category;
        public readonly int Level;
        
        public LevelFinishEvent(int category, int level) : base(0)
        {
            Category = category;
            Level = level;
        }
    }
}