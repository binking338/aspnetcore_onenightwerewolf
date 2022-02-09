using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Seer
{
    public class SeerSeePlayerOption : IOption
    {
        public string Name => "预言家查看玩家";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SeeOthersCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            for (var i = 0; i < table.Seats.Length; i++)
            {
                if (seat.No == table.Seats[i].No) continue;
                choices.Add($"{table.Seats[i].Player}", new Choice(table.Round.Phase, table.Round.Name, $"{table.Seats[i].Player}", new Dictionary<string, string>() {
                        { "Option", $"{Name}"},
                        { "SeeOthersCard", $"{table.Seats[i].Player}"}
                    }));
            }
            return choices;
        }
    }

    public class SeerSeeGraveOption : IOption
    {
        public string Name => "预言家查看中间牌堆";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SeeGraveCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            for (var i = 0; i < table.Graves.Length - 1; i++)
            {
                for (int j = i+1; j < table.Graves.Length; j++)
                {
                    choices.Add($"{table.Graves[i].No}和{table.Graves[j].No}号中间牌", new Choice(table.Round.Phase, table.Round.Name, $"{table.Graves[i].No}和{table.Graves[j].No}号中间牌", new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SeeGraveCard", $"{table.Graves[i].No},{table.Graves[j].No}"}
                    }));
                }
            }
            return choices;
        }
    }
}
