using System;
namespace OneNightWerewolf
{
    public class GameOption
    {
        public GameOption()
        {
        }

        public GamePhase Phase { get; set; }

        public GameCommand Command { get; set; }

        public object[] Arguments { get; set; }

        public object[] Result { get; set; }
    }
}
