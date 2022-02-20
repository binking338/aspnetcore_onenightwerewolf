using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Tanner
{
    public class TannerCard : ICard
    {
        public string Id { get; set; }

        public string No => "11";

        public string Name => "皮匠";

        public Role Role => Role.Tanner;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility()
        };

        public IRound[] Rounds => new IRound[0];
    }
}
