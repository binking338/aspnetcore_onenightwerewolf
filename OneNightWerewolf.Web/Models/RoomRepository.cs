using System;
using System.Collections.Generic;
using System.Linq;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Web.Models
{
    public class RoomRepository
    {
        public RoomRepository()
        {
            roomStorage = new Dictionary<string, Room>();
        }

        private Dictionary<string, Room> roomStorage;

        public bool Exist(string roomId)
        {
            return roomStorage.ContainsKey(roomId);
        }

        public Room Get(string roomId)
        {
            Room room = null;
            if (Exist(roomId)) room = roomStorage[roomId];
            return room;
        }

        public List<Room> List()
        {
            return roomStorage.Values
                .OrderByDescending(room => room.GetTable().StartTime?.Ticks ?? 0)
                .ThenBy(room => room.Name)
                .ToList();
        }

        public Room New(string roomId, string cardNos)
        {
            if (Exist(roomId))
            {
                throw new InvalidOperationException("房间号已经被使用");
            }

            Room room = new Room(roomId, Rounds.AllBasics, ActionHandlers.All, new DefaultWinningCampDecisionRule());
            room.Config(Cards.CreateCards(cardNos));
            roomStorage[roomId] = room;
            return room;
        }

        public bool Remove(string roomId)
        {
            if (!Exist(roomId)) return false;
            return roomStorage.Remove(roomId);
        }
    }
}
