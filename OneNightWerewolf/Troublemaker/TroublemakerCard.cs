using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Troublemaker
{
    public class TroublemakerCard : ICard
    {
        public string No => "07";

        public string Name => "捣蛋鬼";

        public Role Role => Role.Troublemaker;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new TroublemakerAbility() };

        public IRound[] Rounds => new IRound[] { new TroublemakerRound() };
    }
}
