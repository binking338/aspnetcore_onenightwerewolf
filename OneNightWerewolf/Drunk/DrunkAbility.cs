using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Drunk
{
    public class DrunkAbility : IAbility
    {
        public string Name => "酒鬼替换身份";

        public IOption[] Options => new IOption[] { new DrunkOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is DrunkRound);
    }
}
