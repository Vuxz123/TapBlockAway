using System.Collections.Generic;

namespace com.ethnicthv.Game
{
    public class PlayerBadgeChangeEvent : Event
    {
        public readonly IReadOnlyList<int> Badges;
        
        public PlayerBadgeChangeEvent(IReadOnlyList<int> badges) : base(10)
        {
            Badges = badges;
        }
    }
}