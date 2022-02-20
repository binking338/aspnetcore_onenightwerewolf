using OneNightWerewolf.Common;
using OneNightWerewolf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNightWerewolf
{
    public class RoomSerializer
    {
        public string Serialize(Room room)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["room"] = System.Text.Json.JsonSerializer.Serialize(room);
            dic["game"] = System.Text.Json.JsonSerializer.Serialize(room.GetGame().Cards.Select(s => s.No).ToList());
            dic["table"] = System.Text.Json.JsonSerializer.Serialize(room.GetTable());
            dic["seats"] = System.Text.Json.JsonSerializer.Serialize(room.GetTable().GetSeats());
            dic["graves"] = System.Text.Json.JsonSerializer.Serialize(room.GetTable().GetGraves());
            string serialized = System.Text.Json.JsonSerializer.Serialize(dic);
            return serialized;
        }

        public Room Deserialize(string serialized)
        {
            var dic = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(serialized);
            var room = System.Text.Json.JsonSerializer.Deserialize<Room>(dic["room"]);
            var gameConfig = string.Join(",", System.Text.Json.JsonSerializer.Deserialize<List<string>>(dic["game"]));

            var table = System.Text.Json.JsonSerializer.Deserialize<Table>(dic["table"]);
            var seats = System.Text.Json.JsonSerializer.Deserialize<List<Seat>>(dic["seats"]);
            var graves = System.Text.Json.JsonSerializer.Deserialize<List<Grave>>(dic["graves"]);

            room.Config(Cards.CreateCards(gameConfig));

            room.GetTable().Clone(table);
            for (var i = 0; i < room.GetTable().GetGraves().Length; i++)
            {
                room.GetTable().GetGraves()[i].Clone(graves[i]);
            }
            for (var i = 0; i < room.GetTable().GetSeats().Length; i++)
            {
                room.GetTable().GetSeats()[i].Clone(seats[i]);
            }
            return room;
        }
    }
}
