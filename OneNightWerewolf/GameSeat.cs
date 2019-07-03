using System;
namespace OneNightWerewolf
{
    public class GameSeat
    {
        public GameSeat()
        {
        }

        public int No { get; set; }

        public GameCard Card { get; set; }

        public int Vote { get; set; }

        public bool Dead { get; set; }

        public DeadReason DeadReason { get; set; }

        public bool Win { get; set; }

        public GameOption Option { get; set; }
    }
}
