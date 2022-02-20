using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;
using OneNightWerewolf.Web.Models;

namespace OneNightWerewolf.Web.Controllers.Api
{
    // todo 请求校验集中
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        UserRepository userRepository;
        RoomRepository roomRepository;
        public GameController(UserRepository userRepository, RoomRepository roomRepository)
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
        }

        private User GetUser()
        {
            return Models.User.GetUser(HttpContext);
        }

        #region 房间管理

        /// <summary>
        /// 房间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<List<RoomInfo>> RoomList()
        {
            User user = GetUser();
            if (user == null)
                return Response<List<RoomInfo>>.Error(1, "未登录用户");
            var result = roomRepository.List().Select(r => ToRoomInfo(r)).ToList();
            return Response<List<RoomInfo>>.Return(result);
        }

        /// <summary>
        /// 房间新建
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> RoomNew(
            string roomId,
            [FromBody] String[] cards)
        {
            User user = GetUser();
            if (user == null)
            {
                return Response<bool>.Error(1, "未登录用户");
            }
            if (!string.IsNullOrEmpty(user.RoomId) && roomId != user.RoomId)
            {
                return Response<bool>.Error(5, $"当前在房间[{user.RoomId}]中");
            }

            if (cards == null || cards.Length < 3)
            {
                return Response<bool>.Error(12, "卡牌数不能小于3");
            }
            if (roomRepository.Exist(roomId))
            {
                var room = roomRepository.Get(roomId);
                if (room.GetHolder()?.Id != user.Id)
                {
                    return Response<bool>.Error(2, "房间已存在，你不是该房间房主");
                }
                else
                {
                    if (!room.Config(OneNightWerewolf.Common.Cards.CreateCards(string.Join(',', cards))))
                    {
                        return Response<bool>.Error(11, "游戏中不能换卡");
                    }
                    return Response<bool>.Return(true);
                }
            }
            else
            {
                var room = roomRepository.New(roomId, string.Join(',', cards));
                room.TakeIn(new Core.Player(user.Id, user.Nick));
                user.RoomId = roomId;
            }
            return Response<bool>.Return(true);
        }

        /// <summary>
        /// 房间销毁
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> RoomDestroy(string roomId)
        {
            if (!roomRepository.Exist(roomId))
            {
                return Response<bool>.Return(false);
            }

            var room = roomRepository.Get(roomId);
            var players = room.Players;
            foreach (var player in players)
            {
                if (player != null)
                {
                    var user = userRepository.Get(player.Id);
                    user.RoomId = null;
                }
            }
            return Response<bool>.Return(roomRepository.Remove(roomId));
        }

        /// <summary>
        /// 清空房间（连房主一起）
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> RoomClean([FromQuery] string roomId)
        {
            var room = roomRepository.Get(roomId);
            var players = room.Players;
            foreach (var player in players)
            {
                if (player != null)
                {
                    room.TakeOut(player);
                    var kickUser = userRepository.Get(player.Id);
                    if (kickUser != null)
                    {
                        if (!string.IsNullOrEmpty(kickUser.RoomId))
                        {
                            kickUser.RoomId = null;
                        }
                    }
                }
            }
            return Response<bool>.Return(true);
        }

        #endregion

        #region 房间玩家控制

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> Join(string roomId)
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (!roomRepository.Exist(roomId))
                return Response<bool>.Error(3, "房间号不存在");
            if (user.RoomId == roomId)
                return Response<bool>.Error(4, $"已进入该房间[{roomId}]");
            if (!string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(5, $"当前在房间[{user.RoomId}]中");
            if (!string.IsNullOrWhiteSpace(user.RoomId))
            {
                var originRoom = roomRepository.Get(user.RoomId);
                originRoom.TakeOut(new Core.Player(user.Id, user.Nick));
                user.RoomId = null;
            }
            var room = roomRepository.Get(roomId);
            var result = room.TakeIn(new Core.Player(user.Id, user.Nick));
            if (!result)
                return Response<bool>.Error(6, $"房间[{roomId}]满员了");
            user.RoomId = roomId;
            return Response<bool>.Return(result);
        }

        /// <summary>
        /// 位置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<PositionInfo> Position()
        {
            User user = GetUser();
            if (user == null) return Response<PositionInfo>.Return(new PositionInfo() { RoomId = null, IsHolder = false });
            if (string.IsNullOrEmpty(user.RoomId)) return Response<PositionInfo>.Return(new PositionInfo() { RoomId = null, IsHolder = false });
            var room = roomRepository.Get(user.RoomId);
            return Response<PositionInfo>.Return(new PositionInfo()
            {
                RoomId = room?.Name,
                IsHolder = room?.GetHolder()?.Id == user.Id
            });
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> Leave()
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            if (!room.HasPlayer(user.Id))
            {
                user.RoomId = null;
                return Response<bool>.Error(8, $"你已不在房间[{user.RoomId}]中");
            }
            room.TakeOut(room.FindPlayer(user.Id));
            user.RoomId = null;
            return Response<bool>.Return(true);
        }

        /// <summary>
        /// 同房间玩家列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<PlayerInfo[]> Buddies()
        {
            User user = GetUser();
            if (user == null)
                return Response<PlayerInfo[]>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<PlayerInfo[]>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            var player = room.FindPlayer(user.Id);
            if (player == null)
                return Response<PlayerInfo[]>.Error(8, $"不在房间[{user.RoomId}]中");
            var playerInfos = room.Players.Where(s => !string.IsNullOrWhiteSpace(s.Id))
                .Select(p => new PlayerInfo() { Id = p.Id, Nick = p.Nick })
                .ToArray();
            return Response<PlayerInfo[]>.Return(playerInfos);
        }

        /// <summary>
        /// 踢人（房主功能）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<bool> Kick([FromQuery] string id)
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            if (room.GetHolder()?.Id != user.Id)
                return Response<bool>.Error(13, $"只有房主能踢人");
            if (room.GetHolder().Id == id)
                return Response<bool>.Error(14, $"房主不能被踢");

            var player = room.FindPlayer(id);

            if (player != null)
            {
                room.TakeOut(player);
                var kickUser = userRepository.Get(player.Id);
                if (kickUser != null)
                {
                    if (!string.IsNullOrEmpty(kickUser.RoomId))
                    {
                        kickUser.RoomId = null;
                    }
                }
            }
            return Response<bool>.Return(true);
        }

        #endregion

        #region 游戏控制

        [HttpPost]
        public Response<bool> Start()
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            if (room.GetHolder()?.Id != user.Id)
                return Response<bool>.Error(10, "房主才能开始游戏");
            if (!room.GetTable().IsAllSeatReady())
                return Response<bool>.Error(9, "人数不齐或未准备");
            if (!room.GetTable().IsGameFinished())
                return Response<bool>.Return(true);
            return Response<bool>.Return(room.Start());
        }

        [HttpPost]
        public Response<bool> Over()
        {
            User user = GetUser();
            if (user == null)
                return Response<bool>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<bool>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            if (room.GetHolder()?.Id != user.Id)
                return Response<bool>.Error(15, "房主才能结束游戏");
            if (room.GetTable().IsGameFinished())
                return Response<bool>.Return(true);
            room.ForceStop();
            return Response<bool>.Return(true);
        }

        #endregion

        #region 游戏操作

        /// <summary>
        /// 卡牌No->Name映射
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<Dictionary<string, string>> CardMap()
        {
            Dictionary<string, string> map = OneNightWerewolf.Common.Cards.AllCards().ToDictionary(i => i.No, i => i.Name);
            return Response<Dictionary<string, String>>.Return(map);
        }

        /// <summary>
        /// 当前房间卡牌设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<string[]> Cards()
        {
            User user = GetUser();
            if (user == null)
                return Response<string[]>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<string[]>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            var roles = room.GetGame().Cards.Select(card => card.No).ToArray();
            return Response<string[]>.Return(roles);
        }

        /// <summary>
        /// 待选项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<ChoicesInfo> Choices()
        {
            User user = GetUser();
            if (user == null)
                return Response<ChoicesInfo>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<ChoicesInfo>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            var player = room.FindPlayer(user.Id);
            if (player == null)
                return Response<ChoicesInfo>.Error(8, $"不在房间[{user.RoomId}]中");
            // 是否已行动（做了决定）
            var seat = room.GetTable().FindSeatByNick(user.Nick);
            var choiceMaded = room.GetTable().SeatChoices.ContainsKey(seat.No);
            var choices = choiceMaded ? null : room.Choices(player);
            ChoicesInfo result = ToChoicesInfo(room, choices?.Keys.ToArray());
            result.Messages = result.Game.InGame ? GetMessageInfos(room, room.GetTable().FindSeatByNick(player.Nick).No)
                .Where(m => m.Round.Index == result.Round.Index).ToArray() : new MessageInfo[0];
            return Response<ChoicesInfo>.Return(result);
        }

        /// <summary>
        /// 屏幕
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Response<List<MessageInfo>> Screen()
        {
            User user = GetUser();
            if (user == null)
                return Response<List<MessageInfo>>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<List<MessageInfo>>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            var player = room.FindPlayer(user.Id);
            if (player == null)
                return Response<List<MessageInfo>>.Error(8, $"不在房间[{user.RoomId}]中");

            var messages = GetMessageInfos(room, room.GetTable().FindSeatByNick(player.Nick).No);

            return Response<List<MessageInfo>>.Return(messages);
        }

        /// <summary>
        /// 行动
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<ChoicesInfo> Action([FromQuery] string choice)
        {
            User user = GetUser();
            if (user == null)
                return Response<ChoicesInfo>.Error(1, "未登录用户");
            if (string.IsNullOrEmpty(user.RoomId))
                return Response<ChoicesInfo>.Error(7, $"不在任何房间中");
            var room = roomRepository.Get(user.RoomId);
            var player = room.FindPlayer(user.Id);
            if (player == null)
                return Response<ChoicesInfo>.Error(8, $"不在房间[{user.RoomId}]中");

            ChoicesInfo result = ToChoicesInfo(room, new string[] { choice });

            var choices = room.Choices(player);
            room.Action(player, choices[choice]);
            result.Messages = result.Game.InGame ? GetMessageInfos(room, room.GetTable().FindSeatByNick(player.Nick).No)
                .Where(m => m.Round.Index == result.Round.Index).ToArray() : new MessageInfo[0];

            return Response<ChoicesInfo>.Return(result);
        }

        private RoomInfo ToRoomInfo(Room room)
        {
            var roomInfo = new RoomInfo()
            {
                Id = room.Name,
                Cards = room.GetGame().Cards.Select(c => c.No).ToArray(),
                PlayerLimit = room.GetPlayerLimit(),
                Players = room.Players.ToArray(),
            };
            return roomInfo;
        }

        private ChoicesInfo ToChoicesInfo(Room room, string[] choices)
        {
            ChoicesInfo choicesInfo = new ChoicesInfo();
            choicesInfo.Game = ToGameInfo(room);
            choicesInfo.Round = ToRoundInfo(room);
            choicesInfo.Choices = choices;
            choicesInfo.Messages = new MessageInfo[0];
            return choicesInfo;
        }

        private GameInfo ToGameInfo(Room room)
        {
            var gameInfo = new GameInfo()
            {
                InGame = !room.GetTable().IsGameFinished(),
                StartTime = ToTimestamp(room.GetTable().StartTime),
                DawnTime = ToTimestamp(room.GetTable().DawnTime),
                OverTime = ToTimestamp(room.GetTable().OverTime)
            };
            return gameInfo;
        }

        private long? ToTimestamp(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            return ((dateTime.Value.Ticks - new DateTime(1970,1,1).ToLocalTime().Ticks) / 10000000); // 精确毫秒
        }

        private RoundInfo ToRoundInfo(Room room)
        {
            var roundInfo = (room?.GetTable()?.GetRound() != null) ?
                new RoundInfo()
                {
                    Index = room.GetTable().RoundIndex,
                    Phase = room.GetTable().GetRound().Phase,
                    PhaseName = room.GetTable().GetRound().Phase.Readable(),
                    Order = room.GetTable().GetRound().Order,
                    Name = room.GetTable().GetRound().Name
                } : new RoundInfo();
            return roundInfo;
        }

        private List<MessageInfo> GetMessageInfos(Room room, string seatNo)
        {
            var messages = new List<MessageInfo>();
            messages.AddRange(room.GetTable().Monitor.Messages.Select(msg => ToMessageInfo("GM", msg)));
            messages.AddRange(room.GetTable().FindSeat(seatNo).Monitor.Messages.Select(msg => ToMessageInfo("", msg)));
            messages = messages.OrderBy(m => m.Time).ToList();
            return messages;
        }

        private MessageInfo ToMessageInfo(string channel, IMonitor.Message message)
        {
            var messageInfo = new MessageInfo() {
                Channel = channel,
                Time = ToTimestamp(message.Time).Value,
                Content = message.Content,
                Round = new RoundInfo()
                {
                    Index = message.RoundIndex,
                    Phase = message.Phase,
                    PhaseName = message.Phase.Readable(),
                    Order = message.RoundOrder,
                    Name = message.RoundName
                }
            };
            return messageInfo;
        }

        #endregion
    }
}
