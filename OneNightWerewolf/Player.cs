using System;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class Player
    {
        public Player()
        {
            HistoryOptions = new List<GameOption>();
        }

        public int SeatNo { get; set; }

        public string UserId { get; set; }

        public string UserNick { get; set; }

        public List<GameOption> HistoryOptions { get; set; }
    }
}
