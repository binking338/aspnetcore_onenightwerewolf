using System;
using System.Collections.Generic;
using System.Linq;
using OneNightWerewolf.Common;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Console
{
    class Program
    {
        static Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
        static void Main(string[] args)
        {
            Room room = new Room("test001", Rounds.AllBasics, ActionHandlers.All);
            room.Config(Cards.CreateCards("01,01,01,02,02,03,04,04,05,06,07,08,09,10,11,12"));

            List<Player> users = new List<Player>();
            for (var i = 0; i < room.PlayerLimit; i++)
            {
                Player user = new Player($"136758890{i.ToString("00")}", $"user{i}");
                users.Add(user);
            }

            // 
            for (var i=0; i < room.PlayerLimit; i++)
            {
                room.TakeIn(users[i]);
            }

            room.Table.Monitor = new ConsoleMonitor("public");
            for (var i = 0; i < room.PlayerLimit; i++)
            {
                room.Table.Seats[i].Monitor = new ConsoleMonitor(users[i].Nick);
            }

            for(var t =0; t < 100; t++)
            {

                //
                room.Start();

                //
                do
                {
                    for (var i = 0; i < room.PlayerLimit; i++)
                    {
                        var choices = room.Choices(users[i]);
                        System.Console.WriteLine($"{users[i].Nick} 选项开始:");
                        foreach (var choice in choices)
                        {
                            System.Console.WriteLine($"    {choice.Key}");
                        }
                        System.Console.WriteLine($"{users[i].Nick} 选项结束:");
                        var choiceIndex = random.Next(choices.Keys.Count);
                        room.Action(users[i], choices.Values.ToList()[choiceIndex]);
                    }

                }
                while (!room.Table.IsGameFinished());
                //room.ForceStop();
            }

            System.Console.ReadLine();
        }
    }
}
