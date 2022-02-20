using System;
using System.Linq;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Hunter
{
    public class HunterRound : IRound
    {
        public Phase Phase => Phase.Vote;

        public int Order => (int)Role.Hunter;

        public string Name => "猎人开枪";

        public bool Enabled(Table table)
        {
            if (table.GetSeats().Any(s => s.GetFinalCard().Role == Role.Hunter && s.Dead))
            {
                return true;
            }
            return false;
        }
    }
}
