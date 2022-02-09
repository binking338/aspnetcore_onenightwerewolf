using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public class GameReplay
    {
        public GameReplay()
        {
        }

        public IList<GameRoundHistory> Histories { get; } = new List<GameRoundHistory>();
    }

    public class GameRoundHistory
    {
        public GameRoundHistory(IRound round, IDictionary<string, Choice> playerChoices)
        {
            Round = round;
            PlayerChoices = playerChoices;
        }

        public IRound Round { get; private set; }

        public IDictionary<string, Choice> PlayerChoices { get; private set; } 
    }
}
