using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OneNightWerewolf.Web.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OneNightWerewolf.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserProvider userProvider;
        public UserController(UserProvider userProvider)
        {
            this.userProvider = userProvider;
        }

        private string GetClientId()
        {
            var sessionId = HttpContext.Session.Id;
            return sessionId;
        }

        [HttpPost]
        public async Task<Response<bool>> SignIn(string nick)
        {
            var claims = new List<Claim>
                {
                    new Claim("user", GetClientId()),
                    new Claim("role", "player"),
                    new Claim("nick", nick),
                };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")), new AuthenticationProperties() { IsPersistent = true });

            User user = new User()
            {
                Id = GetClientId(),
                Nick = nick
            };
            userProvider.Set(user);
            HttpContext.Session.Set("user", System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(user)));

            return Response<bool>.Return(true);
        }

        [HttpPost]
        public async Task<Response<bool>> Logout()
        {
            await HttpContext.SignOutAsync();
            return Response<bool>.Return(true);
        }

        [HttpGet]
        public Response<User> Info()
        {
            var user = userProvider.Get(GetClientId());
            return Response<User>.Return(user);
        }
    }
}
