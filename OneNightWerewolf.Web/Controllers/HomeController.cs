using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneNightWerewolf.Web.Models;

namespace OneNightWerewolf.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserProvider userProvider;
        public HomeController(UserProvider userProvider)
        {
            this.userProvider = userProvider;
        }

        private string GetUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        private User GetUser()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = userProvider.Get(userId);
            if (user == null)
            {
                user = new User()
                {
                    Id = userId,
                    Nick = HttpContext.User.FindFirst(c => c.Type == "nick")?.Value
                };
                if (string.IsNullOrEmpty(user.Nick))
                {
                    user.Nick = "游客" + (DateTime.Now.Ticks / 1000).ToString();
                }
                userProvider.Set(user);
            }
            return user;
        }

        public IActionResult Index([FromServices] UserProvider userProvider, [FromServices] Game game)
        {
            var user = GetUser();
            if (user != null && !string.IsNullOrEmpty(user.RoomId)  && game.RoomProvider.Exist(user.RoomId)) {
                game.SetRoomId(user.RoomId);
                ViewData["room.id"] = user.RoomId;
                ViewData["room.roles"] = string.Join(",", game.GetAllCards()?.Select(card => (int)card.Role));
            }
            return View(user);
        }

        public IActionResult Intro()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
