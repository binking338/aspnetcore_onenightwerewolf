using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Minion
{
    public class MinionRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Minion;

        public string Name => "爪牙醒来";
    }
}
