using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Robber
{
    public class RobberAbility : IAbility
    {
        public string Name => "强盗交换身份";

        public IOption[] Options => new IOption[] { new RobberOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is RobberRound);
    }
}
