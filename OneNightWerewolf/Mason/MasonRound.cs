using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Mason
{
    public class MasonRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Mason;

        public string Name => "守夜人醒来";
    }
}
