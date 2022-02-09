using System;
namespace OneNightWerewolf.Core
{
    public class Player
    {
        public Player(string id, string nick)
        {
            Id = id;
            Nick = nick;
        }

        public string Id { get; private set; }

        public string Nick { get; private set; }
    }
}
