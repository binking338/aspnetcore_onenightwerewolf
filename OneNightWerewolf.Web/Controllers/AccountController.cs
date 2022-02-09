using System;
using Microsoft.AspNetCore.Mvc;

namespace OneNightWerewolf.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
