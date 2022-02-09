using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Werewolf
{
    public class WerewolfRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Werewolf;

        public string Name => "狼人醒来";
    }
}
