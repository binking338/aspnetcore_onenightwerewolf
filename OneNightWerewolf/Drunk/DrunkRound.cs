using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Drunk
{
    public class DrunkRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Drunk;

        public string Name => "酒鬼醒来";
    }
}
