using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Robber
{
    public class RobberRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Robber;

        public string Name => "强盗醒来";
    }
}
