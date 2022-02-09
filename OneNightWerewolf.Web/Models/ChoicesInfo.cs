using System;
namespace OneNightWerewolf.Web.Models
{
    public class ChoicesInfo
    {
        public GameInfo Game { get; set; }

        public RoundInfo Round { get; set; }

        public string[] Choices { get; set; }
    }

    public class GameInfo
    {
        public bool InGame { get; set; }

        public long? StartTime { get; set; }

        public long? DawnTime { get; set; }

        public long? OverTime { get; set; }
    }

    public class RoundInfo
    {
        public int Index { get; set; }

        public Core.Phase Phase { get; set; }

        public string PhaseName { get; set; }

        public string Name { get; set; }
    }
}
