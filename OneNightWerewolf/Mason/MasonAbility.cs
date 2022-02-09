using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Mason
{
    public class MasonAbility : IAbility
    {
        public string Name => "石匠查看同伴";

        public IOption[] Options => new IOption[] { new MasonOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is MasonRound);
    }
}
