using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Minion
{
    public class MinionCard : ICard
    {
        public string Id { get; set; }

        public string No => "03";

        public string Name => "爪牙";

        public Role Role => Role.Minion;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new MinionAbility()
        };

        public IRound[] Rounds => new IRound[] { new MinionRound() };
    }
}
