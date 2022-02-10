using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Robber
{
    public class RobberOption : IOption
    {
        public string Name => "强盗交换身份";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SwapWithOthers, Core.Action.SeeMyCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            for (var i = 0; i < table.Seats.Length; i++)
            {
                if (seat.No == table.Seats[i].No) continue;
                choices.Add($"{table.Seats[i].Player}", new Choice(table.Round.Phase, table.Round.Name, $"{table.Seats[i].Player}", new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SwapWithOthers", $"{table.Seats[i].No}"}
                    }));
            }
            return choices;
        }
    }
}
