using System;
using System.Collections.Generic;
using System.Linq;
namespace OneNightWerewolf
{
    public class GameRoomProvider
    {
        public GameRoomProvider()
        {
            gameRoomStorage = new Dictionary<string, GameRoom>();       
        }

        public Dictionary<string, GameRoom> gameRoomStorage;

        public bool Exist(string roomId)
        {
            return gameRoomStorage.ContainsKey(roomId);

        }

        public GameRoom Get(string roomId)
        {
            GameRoom room = null;
            if (Exist(roomId)) room = gameRoomStorage[roomId];
            return room;
        }

        public List<GameRoom> List()
        {
            return gameRoomStorage.Values
                .OrderByDescending(room => room.StateStack.Count > 0 ? room.StateStack.LastOrDefault().StartTime?.Ticks ?? 0 : 0)
                .ThenBy(room=>room.Id)
                .ToList();
        }

        public GameRoom New(string roomId, GameRole[] roles)
        {
            if (Exist(roomId)) return null;
            var cards = roles.Select(role => new GameCard() { Role = role }).OrderBy(card => card.Role).ToList();
            cards.Sort(new Comparison<GameCard>((a, b) => a.Role - b.Role));
            GameRoom room = new GameRoom()
            {
                Id = roomId,
                Cards = cards
            };
            gameRoomStorage[roomId] = room;
            return room;
        }

        public bool Remove(string roomId)
        {
            if (!Exist(roomId)) return false;
            return gameRoomStorage.Remove(roomId);
        }
    }
}
