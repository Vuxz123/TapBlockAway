namespace com.ethnicthv.Game.Impl
{
    /// <summary>
    /// This event is used to notify that a new category has been unlocked.
    /// </summary>
    public class UnlockNewCategoryEvent : Event
    {
        public readonly int FromCategory;
        public readonly int FromLevel;
        public readonly int NewCategory;
        public readonly bool IsUpdateByCondition;

        public UnlockNewCategoryEvent(int fromCategory, int formLevel, int newCategory, bool isUpdateByCondition = true)
            : base(1)
        {
            FromCategory = fromCategory;
            FromLevel = formLevel;
            NewCategory = newCategory;
            IsUpdateByCondition = isUpdateByCondition;
        }
    }
}