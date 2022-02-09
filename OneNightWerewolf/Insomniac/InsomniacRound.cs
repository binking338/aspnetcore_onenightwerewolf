using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Insomniac
{
    public class InsomniacRound : IRound
    {
        public Phase Phase => Phase.Night;

        public int Order => (int)Role.Insomniac;

        public string Name => "失眠者醒来";
    }
}
