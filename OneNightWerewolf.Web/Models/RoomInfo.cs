using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Web.Models
{
    public class RoomInfo
    {
        public string Id { get; set; }
        public string[] Cards { get; set; }
        public Player[] Players { get; set; }
        public int PlayerLimit { get; set; }

    }
}
