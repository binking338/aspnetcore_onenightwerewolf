using OneNightWerewolf.Common;
using OneNightWerewolf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNightWerewolf
{
    public interface ISerializer<T>
    {
        string Serialize(T t);

        T Deserialize(string s);
    }

    public class RoomSerializer : ISerializer<Room>
    {
        public Room Deserialize(string s)
        {
            throw new NotImplementedException();
        }

        public string Serialize(Room t)
        {
            throw new NotImplementedException();
        }
    }

    public class GameSerializer : ISerializer<Game>
    {
        public Game Deserialize(string cardNos)
        {
            var cards = Cards.CreateCards(cardNos);
            var game = new Game(Rounds.AllBasics, ActionHandlers.All, new DefaultWinningCampDecisionRule());
            game.New(cards);
            return game;
        }

        public string Serialize(Game game)
        {
            string cardNos = string.Join(",",  game.Cards.Select(c => c.No));
            return cardNos;
        }
    }

    public class TableSerializer : ISerializer<Table>
    {
        public Table Deserialize(string s)
        {
            throw new NotImplementedException();
        }

        public string Serialize(Table t)
        {
            throw new NotImplementedException();
        }
    }

    public class SeatSerializer : ISerializer<Seat>
    {
        public Seat Deserialize(string s)
        {
            throw new NotImplementedException();
        }

        public string Serialize(Seat t)
        {
            throw new NotImplementedException();
        }
    }

    public class GraveSerializer : ISerializer<Grave>
    {
        public Grave Deserialize(string s)
        {
            throw new NotImplementedException();
        }

        public string Serialize(Grave t)
        {
            throw new NotImplementedException();
        }
    }
}
