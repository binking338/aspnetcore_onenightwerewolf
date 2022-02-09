using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Doppelganger
{
    public class DoppelgangerRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Werewolf - 50;

        public string Name => "幽灵醒来";
    }
}
