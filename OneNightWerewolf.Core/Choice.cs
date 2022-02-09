using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public class Choice
    {
        public Choice(Phase phase, string roundName, string title, IDictionary<string, string> data)
        {
            Phase = phase;
            RoundName = roundName;
            Title = title;
            Data = data;
        }

        public Phase Phase { get; }
        public string RoundName { get; }
        public string Title { get; }
        public IDictionary<string, string> Data { get; }
    }
}
