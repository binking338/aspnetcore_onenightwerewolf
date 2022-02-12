using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Hunter
{
    public class HunterOption : IOption
    {
        public string Name => "猎人开枪";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.Hunt };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            if (seat.FinalCard.Role == Role.Hunter && seat.Dead)
            {
                foreach (var s in table.Seats)
                {
                    if (s.No == seat.No) continue;
                    var choice = $"[{s.Player}]";
                    choices.Add(choice, new Choice(table.Round.Phase, table.Round.Name, choice, new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "Hunt", $"{s.No}" }
                    }));
                }
            }
            else
            {
                var choice = "不能开枪";
                choices.Add(choice, new Choice(table.Round.Phase, table.Round.Name, choice, new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "Hunt", $"" }
                }));
            }
            return choices;
        }
    }
}
