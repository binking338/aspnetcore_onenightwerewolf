using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Werewolf
{
    public class WerewolfCard : ICard
    {
        public string No => "02";

        public string Name => "狼人";

        public Role Role => Role.Werewolf;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new WerewolfAbility()
        };

        public IRound[] Rounds => new IRound[] { new WerewolfRound() };
    }
}
