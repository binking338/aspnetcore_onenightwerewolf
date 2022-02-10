using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Troublemaker
{
    public class TroublemakerOption : IOption
    {
        public string Name => "捣蛋鬼交换玩家手牌";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SwapOthers };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            for (var i = 0; i < table.Seats.Length - 1; i++)
            {
                for (int j = i+1; j < table.Seats.Length; j++)
                {
                    choices.Add($"{table.Seats[i].Player}和{table.Seats[j].Player}", new Choice(table.Round.Phase, table.Round.Name, $"{table.Seats[i].Player}和{table.Seats[j].Player}", new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SwapOthers", $"{table.Seats[i].No},{table.Seats[j].No}"}
                    }));
                }
            }
            return choices;
        }
    }
}
