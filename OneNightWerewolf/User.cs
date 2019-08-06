using System;
namespace OneNightWerewolf
{
    public class User
    {
        public User()
        {
        }

        public string Id { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public string Nick { get; set; }

        public string RoomId { get; set; }
    }
}
