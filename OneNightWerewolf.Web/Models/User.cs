using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OneNightWerewolf.Web.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public string Nick { get; set; }

        public string RoomId { get; set; }

        public static string GetUserId(HttpContext httpContext)
        {
            return httpContext.User.Identity.Name;
        }

        public static User GetUser(HttpContext httpContext)
        {
            var userId = GetUserId(httpContext);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var userRepository = httpContext.RequestServices.GetService<UserRepository>();
            var user =  userRepository.Get(userId);
            if (user == null)
            {
                user = new User()
                {
                    Id = userId,
                    Nick = httpContext.User.FindFirst(c => c.Type == "nick")?.Value
                };
                if (string.IsNullOrEmpty(user.Nick))
                {
                    user.Nick = "游客" + (DateTime.Now.Ticks / 1000).ToString();
                }
                userRepository.Set(user);
            }
            return user;
        }
    }
}
