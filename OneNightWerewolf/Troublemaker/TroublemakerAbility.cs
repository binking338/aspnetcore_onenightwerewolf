using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Troublemaker
{
    public class TroublemakerAbility : IAbility
    {
        public string Name => "捣蛋鬼交换玩家手牌";

        public IOption[] Options => new IOption[] { new TroublemakerOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is TroublemakerRound);
    }
}
