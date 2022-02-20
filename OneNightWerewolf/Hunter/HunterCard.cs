using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Hunter
{
    public class HunterCard : ICard
    {
        public string Id { get; set; }

        public string No => "10";

        public string Name => "猎人";

        public Role Role => Role.Hunter;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new HunterAbility()
        };

        public IRound[] Rounds => new IRound[] { new HunterRound() };
    }
}
