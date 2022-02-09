using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Troublemaker
{
    public class TroublemakerRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Troublemaker;

        public string Name => "捣蛋鬼醒来";
    }
}
