using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public class Choice
    {
        public Choice()
        {
        }

        public Choice(Phase phase, string roundName, string title, Dictionary<string, string> data)
        {
            Phase = phase;
            RoundName = roundName;
            Title = title;
            Data = data;
        }

        public Phase Phase { get; set; }
        public string RoundName { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
