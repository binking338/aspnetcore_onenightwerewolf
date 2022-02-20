using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Village
{
    public class VillageCard : ICard
    {
        public string Id { get; set; }

        public string No => "01";

        public string Name => "村民";

        public Role Role => Role.Village;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility()
        };

        public IRound[] Rounds => new IRound[0];
    }
}
