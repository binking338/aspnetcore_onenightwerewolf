using System;
namespace OneNightWerewolf.Web.Models
{
    public class MessageInfo
    {
        public string Channel { get; set; }

        public long Time { get; set; }

        public string Content { get; set; }

        public RoundInfo Round { get; set; }
    }
}
