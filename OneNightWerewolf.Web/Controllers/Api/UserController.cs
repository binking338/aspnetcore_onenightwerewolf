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

        private string GetUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        [HttpPost]
        public async Task<Response<bool>> SignIn(string nick, string password)
        {
            if(nick.Length > 20 || password.Length > 20)
            {
                // 名字密码太长
                return Response<bool>.Error(-1, "用户名和密码不能超过20个字符！");
            }
            var userId = nick;
            User user = userProvider.Get(userId);
            if (userProvider.Get(userId) != null)
            {
                if (user.ClientId != GetClientId() && user.Password != password)
                {
                    return Response<bool>.Error(-2, "密码不正确！");
                }
            }

            var claims = new List<Claim>
                {
                    new Claim("user", userId),
                    new Claim("role", "player"),
                    new Claim("nick", nick),
                };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")), new AuthenticationProperties() { IsPersistent = true });

            user = new User()
            {
                Id = userId,
                Password = password,
                ClientId = GetClientId(),
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
            var user = userProvider.Get(GetUserId());
            return Response<User>.Return(user);
        }
    }
}
