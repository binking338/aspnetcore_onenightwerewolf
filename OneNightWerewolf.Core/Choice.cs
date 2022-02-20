using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public class Choice
    {
        public Choice(Phase phase, string roundName, string title, Dictionary<string, string> data)
        {
            Phase = phase;
            RoundName = roundName;
            Title = title;
            Data = data;
        }

        public Phase Phase { get; private set; }
        public string RoundName { get; private set; }
        public string Title { get; private set; }
        public Dictionary<string, string> Data { get; private set; }
    }
}
