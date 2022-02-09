using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Seer
{
    public class SeerCard : ICard
    {
        public string No => "05";

        public string Name => "预言家";

        public Role Role => Role.Seer;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new SeerAbility()
        };

        public IRound[] Rounds => new IRound[] { new SeerRound() };
    }
}
