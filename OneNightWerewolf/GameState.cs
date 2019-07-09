using System;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class GameState
    {
        public GameState()
        {
            Seats = new List<GameSeat>();
        }

        public List<GameSeat> Seats { get; set; }

        public GamePhase Phase { get; set; }

    }
}
