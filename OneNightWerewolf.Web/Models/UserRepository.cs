using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Web.Models
{
    public class UserRepository
    {
        public UserRepository()
        {
            userStorage = new Dictionary<string, User>();
        }
        public Dictionary<string, User> userStorage;


        public User Get(string userId)
        {
            User user = null;
            if (userStorage.ContainsKey(userId)) user = userStorage[userId];
            return user;
        }

        public void Set(User user)
        {
            userStorage[user.Id] = user;
        }
    }
}
