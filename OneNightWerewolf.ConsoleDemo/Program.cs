using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OneNightWerewolf.ConsoleDemo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            GameRoom room = new GameRoom();
            room.Id = "测试房间";
            room.Cards = new System.Collections.Generic.List<GameCard>()
            {
                new GameCard(){Role = GameRole.Village},
                new GameCard(){Role = GameRole.Village},
                new GameCard(){Role = GameRole.Village},
                new GameCard(){Role = GameRole.Werewolf},
                new GameCard(){Role = GameRole.Werewolf},
                new GameCard(){Role = GameRole.Seer},
                new GameCard(){Role = GameRole.Robber},
                new GameCard(){Role = GameRole.Troublemaker},
                new GameCard(){Role = GameRole.Drunk},
                new GameCard(){Role = GameRole.Insomniac},
                new GameCard(){Role = GameRole.Hunter},
                new GameCard(){Role = GameRole.Tanner},
            };
            room.Players = new List<Player>();
            for(var i=0; i < room.Cards.Count - 3; i++)
            {
                room.Players.Add(new Player() { UserId = "player" + i, UserNick = "P" + i, SeatNo = i });
            };

            Console.WriteLine(JsonConvert.SerializeObject(room.Cards));
            Console.WriteLine(JsonConvert.SerializeObject(room.Players));

            Game game = new Game(new GameRoomProvider());
            game.Room = room;
            Random random = new Random(DateTime.Now.TimeOfDay.Milliseconds);
            while (Console.ReadLine() != "exit")
            {
                Console.WriteLine("===================NewGame===================");
                game.Start();
                while (game.GetCurrentPhase() != GamePhase.Over)
                {
                    for(var i=0; i<game.Room.Players.Count; i++)
                    {
                        var options = game.GetOptions(i);
                        Console.WriteLine(JsonConvert.SerializeObject(options));
                        game.Action(i, options[random.Next(options.Count)]);
                    }
                }
                for (var i = 0; i < game.Room.Players.Count; i++)
                {
                    var options = game.GetOptions(i);
                    Console.WriteLine(JsonConvert.SerializeObject(options));
                    game.Action(i, options[random.Next(options.Count)]);
                }

                Console.WriteLine(JsonConvert.SerializeObject(room.StateStack, Formatting.Indented));

                for (var i = 0; i < game.Room.Players.Count; i++)
                {
                    Console.WriteLine(game.GetMyGameInfo(i));
                    Console.ReadLine();
                }
            }
        }
    }
}
