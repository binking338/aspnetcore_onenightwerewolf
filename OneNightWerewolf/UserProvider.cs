using System;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class UserProvider
    {
        public UserProvider()
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
