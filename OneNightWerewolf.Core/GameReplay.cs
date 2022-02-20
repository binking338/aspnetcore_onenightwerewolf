using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public class GameReplay
    {
        public GameReplay()
        {
        }

        public List<GameRoundHistory> Histories { get; set; } = new List<GameRoundHistory>();
    }

    public class GameRoundHistory : IRound
    {
        public GameRoundHistory()
        {

        }

        public GameRoundHistory(IRound round, Dictionary<string, Choice> playerChoices)
        {
            Phase = round.Phase;
            Order = round.Order;
            Name = round.Name;
            PlayerChoices = playerChoices;
        }

        public Phase Phase { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public Dictionary<string, Choice> PlayerChoices { get; set; } 
    }
}
