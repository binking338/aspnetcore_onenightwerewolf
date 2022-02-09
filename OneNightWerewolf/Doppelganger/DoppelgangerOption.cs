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
                choices.Add($"{s.Player}", new Choice(table.Round.Phase, table.Round.Name, $"{s.Player}", new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "SeeOthersCard", $"{s.Player}" },
                    { "CopyOthersCard", $"{s.Player}" }
                }));
            }
            return choices;
        }
    }
}
