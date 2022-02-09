using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Doppelganger
{
    public class DoppelgangerAbility : IAbility
    {
        public string Name => "幽灵化身";

        public IOption[] Options => new IOption[] { new DoppelgangerOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is DoppelgangerRound);
    }
}
