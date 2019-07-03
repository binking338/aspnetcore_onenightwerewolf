using System;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class GameState
    {
        public GameState()
        {
            Seats = new Dictionary<int, GameSeat>();
        }

        public Dictionary<int, GameSeat> Seats { get; set; }

        public GamePhase Phase { get; set; }

    }
}
