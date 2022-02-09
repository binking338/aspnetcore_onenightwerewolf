using System;
namespace OneNightWerewolf.Core
{
    public interface ICard
    {
        string No { get; }

        string Name { get; }

        Role Role { get; }

        IAbility[] Abilities { get; }

        IRound[] Rounds { get; }

        public virtual void Reset()
        {
            ;
        }
    }
}
