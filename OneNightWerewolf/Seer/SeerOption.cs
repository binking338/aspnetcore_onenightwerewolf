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
            for (var i = 0; i < table.GetSeats().Length; i++)
            {
                if (seat.No == table.GetSeats()[i].No) continue;
                var choice = $"查看[{table.GetSeats()[i].Player}]";
                choices.Add(choice, new Choice(table.GetRound().Phase, table.GetRound().Name, choice, new Dictionary<string, string>() {
                        { "Option", $"{Name}"},
                        { "SeeOthersCard", $"{table.GetSeats()[i].No}"}
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
            for (var i = 0; i < table.GetGraves().Length - 1; i++)
            {
                for (int j = i+1; j < table.GetGraves().Length; j++)
                {
                    var choice = $"查看{table.GetGraves()[i].No}和{table.GetGraves()[j].No}号牌";
                    choices.Add(choice, new Choice(table.GetRound().Phase, table.GetRound().Name, choice, new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SeeGraveCard", $"{table.GetGraves()[i].No},{table.GetGraves()[j].No}"}
                    }));
                }
            }
            return choices;
        }
    }
}
