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
            Room room = new Room("test001", Rounds.AllBasics, ActionHandlers.All, new DefaultWinningCampDecisionRule());
            room.Config(Cards.CreateCards("01,01,01,02,02,03,04,04,05,06,07,08,09,10,11,12"));

            List<Player> users = new List<Player>();
            for (var i = 0; i < room.GetPlayerLimit(); i++)
            {
                Player user = new Player($"136758890{i.ToString("00")}", $"user{i}");
                users.Add(user);
            }

            // 
            for (var i=0; i < room.GetPlayerLimit(); i++)
            {
                room.TakeIn(users[i]);
            }

            for(var t =0; t < 100; t++)
            {

                //
                room.Start();

                //
                do
                {
                    for (var i = 0; i < room.GetPlayerLimit(); i++)
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
                while (!room.GetTable().IsGameFinished());


                foreach(var msg in room.GetTable().Monitor.Messages)
                {
                    System.Console.WriteLine($"public : [{msg.Phase }] {msg.Content}");
                }
                for (var i = 0; i < room.GetPlayerLimit(); i++)
                {
                    foreach (var msg in room.GetTable().GetSeats()[i].Monitor.Messages)
                    {
                        System.Console.WriteLine($"seat{room.GetTable().GetSeats()[i].No} : [{msg.Phase }] {msg.Content}");
                    }
                }
                //room.ForceStop();
            }

            System.Console.ReadLine();
        }
    }
}
