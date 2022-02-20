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
        public GameRoundHistory(IRound round, Dictionary<string, Choice> playerChoices)
        {
            Phase = round.Phase;
            Order = round.Order;
            Name = round.Name;
            PlayerChoices = playerChoices;
        }

        public Phase Phase { get; }

        public int Order { get; }

        public string Name { get; }

        public Dictionary<string, Choice> PlayerChoices { get; private set; } 
    }
}
