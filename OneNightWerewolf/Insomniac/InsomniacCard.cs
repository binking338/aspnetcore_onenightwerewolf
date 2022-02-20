using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Insomniac
{
    public class InsomniacCard : ICard
    {
        public string Id { get; set; }

        public string No => "09";

        public string Name => "失眠者";

        public Role Role => Role.Insomniac;

        public IAbility[] Abilities => new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new InsomniacAbility() };

        public IRound[] Rounds => new IRound[] { new InsomniacRound() };
    }
}
