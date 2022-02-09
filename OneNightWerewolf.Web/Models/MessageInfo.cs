using System;
namespace OneNightWerewolf.Web.Models
{
    public class MessageInfo
    {
        public MessageInfo()
        {
        }

        public long Time { get; set; }

        public string Channel { get; set; }

        public string Content { get; set; }
    }
}
