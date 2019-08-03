using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneNightWerewolf.Web.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OneNightWerewolf.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        UserProvider userProvider;
        public GameController(UserProvider userProvider)
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
            if(user == null)
            {
                user = new User()
                {
                    Id = userId,
                    Nick = HttpContext.User.FindFirst(c => c.Type == "nick")?.Value
                };
                if(string.IsNullOrEmpty( user.Nick))
                {
                    user.Nick = "游客" + (DateTime.Now.Ticks / 1000).ToString();
                }
                userProvider.Set(user);
            }
            return user;
        }

        [HttpPost]
        public Response<bool> New(string roomId, [FromBody]GameRole[] roles, [FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<bool>.Error(1, "未登录用户");

            if (roles == null || roles.Length < 3)
            {
                return Response<bool>.Error(12, "角色数不能小于3");
            }
            if (game.RoomProvider.Exist(roomId))
            {
                game.SetRoomId(roomId);
                if(!game.IsRoomMaster(user.Id))
                {
                    return Response<bool>.Error(2, "房间号已存在");
                }
                else
                {

                    if (!game.ChangeCards(roles))
                    {
                        return Response<bool>.Error(11, "游戏中不能换卡");
                    }

                    return Response<bool>.Return(true);
                }
            }
            else
            {
                game.Room = game.RoomProvider.New(roomId, roles);
                game.Join(user);
            }
            return Response<bool>.Return(true);
        }

        [HttpPost]
        public Response<bool> Join(string roomId, [FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<bool>.Error(1, "未登录用户");
            if (!game.RoomProvider.Exist(roomId)) 
                return Response<bool>.Error(3, "房间号不存在");
            if (user.RoomId == roomId) 
                return Response<bool>.Error(4, $"已进入该房间[{roomId}]");
            if (!string.IsNullOrEmpty(user.RoomId)) 
                return Response<bool>.Error(5, $"当前在房间[{user.RoomId}]中");
            var result = game.SetRoomId(roomId).Join(user);
            if(!result) 
                return Response<bool>.Error(6, $"房间[{roomId}]满员了");
            return Response<bool>.Return(result);
        }

        [HttpPost]
        public Response<bool> Leave([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId)) 
                return Response<bool>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            if(game.GetPlayerByUserId(user.Id) == null)
            {
                user.RoomId = null;
                return Response<bool>.Error(8, $"不在房间[{user.RoomId}]中");
            }
            //if (game.GetCurrentPhase() != GamePhase.Over || !game.Leave(user))
            //{
            //    return Response<bool>.Error(16, $"游戏中不能退出房间");
            //}
            game.Leave(user);
            return Response<bool>.Return(true);
        }

        [HttpPost]
        public Response<bool> Kick([FromQuery]int seatNo, [FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            if (!game.IsRoomMaster(user.Id))
                return Response<bool>.Error(13, $"只有房主能踢人");
            if (seatNo == 0)
                return Response<bool>.Error(14, $"房主不能被踢");

            if(seatNo < 0)
            {
                var players = game.GetPlayers();
                for(var i = 1; i < players.Count; i ++)
                {
                    game.Leave(userProvider.Get(players[i].UserId));
                }
            }
            else
            {
                var player = game.GetPlayerBySeatNo(seatNo);
                if(player != null)
                {
                    var kickUser = userProvider.Get(player.UserId);
                    if(kickUser != null && ! string.IsNullOrEmpty(kickUser.RoomId))
                    {
                        game.Leave(kickUser);
                    }
                    else
                    {
                        game.Kick(seatNo);
                    }
                }
            }
            return Response<bool>.Return(true);
        }

        [HttpPost]
        public Response<bool> Start([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))  
                return Response<bool>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            if (!game.IsRoomMaster(user.Id)) 
                return Response<bool>.Error(10, "房主才能开始游戏");
            if (!game.HasEnoughPlayers() || game.Start() == null) 
                return Response<bool>.Error(9, "人数不齐"); 
            return Response<bool>.Return(true);
        }

        [HttpPost]
        public Response<bool> Over([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            if (!game.IsRoomMaster(user.Id))
                return Response<bool>.Error(15, "房主才能结束游戏");
            
            return Response<bool>.Return(true);
        }

        [HttpGet]
        public Response<Player[]> Players([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<Player[]>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<Player[]>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null)
                return Response<Player[]>.Error(8, $"不在房间[{user.RoomId}]中");
            return Response<Player[]>.Return(game.GetPlayers().ToArray());
        }
        [HttpGet]
        public Response<String[]> Roles([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<String[]>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<String[]>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var roles = game.GetAllCards().Select(card => game.GetRoleDesc(card.Role)).ToArray();
            return Response<String[]>.Return(roles);
        }

        [HttpGet]
        public Response<string> MyInfo([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<string>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId)) 
                return Response<string>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null) 
                return Response<string>.Error(8, $"不在房间[{user.RoomId}]中");
            return Response<string>.Return(game.GetMyGameInfo(player.SeatNo));
        }

        [HttpGet]
        public Response<bool> IsRoomMaster([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) return Response<bool>.Return(false);
            if (string.IsNullOrEmpty(user.RoomId)) return Response<bool>.Return(false);
            game.SetRoomId(user.RoomId);
            return Response<bool>.Return(game.IsRoomMaster(user.Id));
        }

        [HttpGet]
        public Response<GameOption[]> MyOptions([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null) 
                return Response<GameOption[]>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId)) 
                return Response<GameOption[]>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null) 
                return Response<GameOption[]>.Error(8, $"不在房间[{user.RoomId}]中");
            var options = game.GetOptions(player.SeatNo);
            return Response<GameOption[]>.Return(options?.ToArray());
        }

        [HttpPost]
        public Response<OptionDescModel> Action([FromQuery]int no, [FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<OptionDescModel>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<OptionDescModel>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null)
                return Response<OptionDescModel>.Error(8, $"不在房间[{user.RoomId}]中");
            var option = game.GetOptions(player.SeatNo)[no];
            option = game.Action(player.SeatNo, option);
            if (option == null) return Response<OptionDescModel>.Return(null);
            OptionDescModel result = new OptionDescModel();
            result.Phase = game.GetPhaseDesc(option.Phase);
            result.Options = new string[] { $"{game.GetOptionDesc(option)}{(option.Result == null ? "" : "=>" + game.GetOptionResultDesc(option))}" };
            return Response<OptionDescModel>.Return(result);
        }

        [HttpGet]
        public Response<int> GetDawnTime([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<int>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<int>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            DateTime? dawnTime = game.GetDawnTime();
            if (dawnTime == null) return Response<int>.Return(-1);
            var duration = (int)(DateTime.Now - dawnTime.Value).TotalSeconds;
            return Response<int>.Return(duration);
        }

        [HttpGet]
        public Response<OptionDescModel> MyOptionsDesc([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<OptionDescModel>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<OptionDescModel>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null)
                return Response<OptionDescModel>.Error(8, $"不在房间[{user.RoomId}]中");
            var options = game.GetOptions(player.SeatNo);
            if (options == null || options.Count == 0) return Response<OptionDescModel>.Return(null);
            OptionDescModel result = new OptionDescModel();
            result.Phase = game.GetPhaseDesc(options.First().Phase);
            result.Options = options.Select(option => $"{game.GetOptionDesc(option)}{(option.Result == null ? "" : "=>" + game.GetOptionResultDesc(option))}").ToArray();
            return Response<OptionDescModel>.Return(result);
        }

        [HttpGet]
        public Response<OptionDescModel> MyLastOptionDesc([FromServices] Game game)
        {
            User user = GetUser();
            if (user == null)
                return Response<OptionDescModel>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<OptionDescModel>.Error(7, $"不在任何房间中");
            game.SetRoomId(user.RoomId);
            var player = game.GetPlayerByUserId(user.Id);
            if (player == null)
                return Response<OptionDescModel>.Error(8, $"不在房间[{user.RoomId}]中");
            var option = player.HistoryOptions.LastOrDefault();
            if (option == null) return Response<OptionDescModel>.Return(null);
            OptionDescModel result = new OptionDescModel();
            result.Phase = game.GetPhaseDesc(option.Phase);
            result.Options = new string[] { $"{game.GetOptionDesc(option)}{(option.Result == null ? "" : "=>" + game.GetOptionResultDesc(option))}" };
            return Response<OptionDescModel>.Return(result);
        }

    }
}
