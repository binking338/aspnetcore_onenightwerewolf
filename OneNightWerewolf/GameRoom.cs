using System;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class GameRoom
    {
        public GameRoom()
        {
            Cards = new List<GameCard>();
            Players = new List<Player>();
            StateStack = new List<GameState>();
        }

        public string Id { get; set; }

        public List<GameCard> Cards { get; set; }

        public List<Player> Players { get; set; }

        public List<GameState> StateStack { get; set; }
    }
}
