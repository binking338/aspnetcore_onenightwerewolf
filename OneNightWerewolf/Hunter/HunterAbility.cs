using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Hunter
{
    public class HunterAbility : IAbility
    {
        public string Name => "猎人开枪";

        public IOption[] Options => new IOption[] { new HunterOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is HunterRound);
    }
}
