using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Minion
{
    public class MinionAbility : IAbility
    {
        public string Name => "爪牙查看狼人";

        public IOption[] Options => new IOption[] { new MinionOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is MinionRound);
    }
}
