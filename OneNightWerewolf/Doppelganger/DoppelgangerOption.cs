using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Doppelganger
{
    public class DoppelgangerOption : IOption
    {
        public string Name => "幽灵化身";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SeeOthersCard, Core.Action.CopyOthersCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            foreach (var s in table.Seats)
            {
                if (s.No == seat.No) continue;
                var choice = $"化身[{s.Player}]";
                choices.Add(choice, new Choice(table.GetRound().Phase, table.GetRound().Name, choice, new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "SeeOthersCard", $"{s.No}" },
                    { "CopyOthersCard", $"{s.No}" }
                }));
            }
            return choices;
        }
    }
}
