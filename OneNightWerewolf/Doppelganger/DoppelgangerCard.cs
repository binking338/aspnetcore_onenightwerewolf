using System;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Doppelganger
{
    public class DoppelgangerCard : ICard
    {
        public string Id { get; set; }

        private ICard card = null;

        public string No => "12";

        public string Name => card != null ? (card.Name + "-化身幽灵") : "化身幽灵";

        public Role Role => card != null ? card.Role : Role.Doppelganger;

        public IAbility[] Abilities => card != null ? card.Abilities : new IAbility[] {
            new StartAbility(), new VoteAbility(), new TicketsAbility(), new JudgeAbility(), new OverAbility(),
            new DoppelgangerAbility()
        };

        public IRound[] Rounds => card != null ? card.Rounds : new IRound[] { new DoppelgangerRound() };

        public void Copy(ICard card)
        {
            this.card = card;
        }

        public void Reset()
        {
            this.card = null;
        }
    }
}