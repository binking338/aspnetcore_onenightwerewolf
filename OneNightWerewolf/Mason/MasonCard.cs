using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Mason
{
    public class MasonCard : ICard
    {
        public string Id { get; set; }

        public string No => "04";

        public string Name => "守夜人";

        public Role Role => Role.Mason;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new MasonAbility()
        };

        public IRound[] Rounds => new IRound[] { new MasonRound() };
    }
}
