using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Werewolf
{
    public class WerewolfAbility : IAbility
    {
        public string Name => "狼人查看同伴";

        public IOption[] Options => new IOption[] { new WerewolfFindBuddyOption(), new WerewolfSeeGraveCardOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is WerewolfRound);
    }
}
