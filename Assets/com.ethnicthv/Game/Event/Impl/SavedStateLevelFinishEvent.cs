namespace com.ethnicthv.Game.Impl
{
    /// <summary>
    /// This event is used to notify that the level has been finished and the state has been saved.
    /// </summary>
    public class SavedStateLevelFinishEvent : Event
    {
        public readonly int Category;
        public readonly int Level;
        public readonly int LevelGroup;
        public readonly int NextLevel;
        public readonly int NextLevelGroup;
        public readonly bool IsLastLevelInGroup;
        public readonly bool IsLastLevelInCategory;

        public SavedStateLevelFinishEvent(int category, int level, bool isLastLevelInGroup, int nextLevel,
            int levelGroup, int nextLevelGroup, bool isLastLevelInCategory = false) 
            : base(3)
        {
            Category = category;
            Level = level;
            NextLevel = nextLevel;
            LevelGroup = levelGroup;
            NextLevelGroup = nextLevelGroup;
            IsLastLevelInGroup = isLastLevelInGroup;
            IsLastLevelInCategory = isLastLevelInCategory;
        }
    }
}