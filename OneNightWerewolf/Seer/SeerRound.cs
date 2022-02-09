using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Seer
{
    public class SeerRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Seer;

        public string Name => "预言家醒来";
    }
}
