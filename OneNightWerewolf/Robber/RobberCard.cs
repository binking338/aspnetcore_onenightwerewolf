using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Robber
{
    public class RobberCard : ICard
    {
        public string Id { get; set; }

        public string No => "06";

        public string Name => "强盗";

        public Role Role => Role.Robber;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new RobberAbility() };

        public IRound[] Rounds => new IRound[] { new RobberRound() };
    }
}
