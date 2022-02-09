using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Seer
{
    public class SeerAbility : IAbility
    {
        public string Name => "预言家预言";

        public IOption[] Options => new IOption[] { new SeerSeePlayerOption(), new SeerSeeGraveOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is SeerRound);
    }
}
