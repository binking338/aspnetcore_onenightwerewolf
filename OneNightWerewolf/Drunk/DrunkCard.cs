using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Drunk
{
    public class DrunkCard : ICard
    {
        public string Id { get; set; }

        public string No => "08";

        public string Name => "酒鬼";

        public Role Role => Role.Drunk;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new DrunkAbility()
        };

        public IRound[] Rounds => new IRound[] { new DrunkRound() };
    }
}
