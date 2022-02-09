using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Common
{
    public static class Rounds
    {
        public static IRound[] AllBasics
        {
            get
            {
                return new IRound[]
                {
                    new StartRound(),
                    new VoteRound(),
                    new TicketsRound(),
                    new JudgeRound(),
                    new OverRound()
                };
            }
        }
    }

    public class StartRound : IRound
    {
        public Phase Phase => Phase.Start;

        public int Order => 0;

        public string Name => Phase.Readable();
    }

    public class VoteRound : IRound
    {
        public Phase Phase => Phase.Vote;

        public int Order => 0;

        public string Name => Phase.Readable();
    }

    public class TicketsRound : IRound
    {
        public Phase Phase => Phase.Vote;

        public int Order => 1;

        public string Name => Phase.Readable() + "统计";
    }

    public class JudgeRound : IRound
    {
        public Phase Phase => Phase.Judge;

        public int Order => 0;

        public string Name => Phase.Readable();
    }

    public class OverRound : IRound
    {
        public Phase Phase => Phase.Over;

        public int Order => 0;

        public string Name => Phase.Readable();
    }
}
