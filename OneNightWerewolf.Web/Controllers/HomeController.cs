using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneNightWerewolf.Web.Models;

namespace OneNightWerewolf.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRepository userProvider;

        public HomeController(ILogger<HomeController> logger, UserRepository userProvider)
        {
            _logger = logger;
            this.userProvider = userProvider;
        }

        private User GetUser()
        {
            return Models.User.GetUser(HttpContext);
        }

        public IActionResult Index([FromServices] UserRepository userProvider, [FromServices] RoomRepository roomRepository)
        {
            var user = GetUser();
            ViewData["user.nick"] = user.Nick;
            if (user != null && !string.IsNullOrEmpty(user.RoomId) && roomRepository.Exist(user.RoomId))
            {
                var room = roomRepository.Get(user.RoomId);
                ViewData["room.id"] = user.RoomId;
                ViewData["room.roles"] = string.Join(",", room.GetGame().Cards?.Select(card => card.No));
            }
            return View(user);
        }

        public IActionResult Admin()
        {
            return View();
        }

        [AllowAnonymous]
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
