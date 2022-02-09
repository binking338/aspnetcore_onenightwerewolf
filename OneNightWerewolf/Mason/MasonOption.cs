using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Mason
{
    public class MasonOption : IOption
    {
        public string Name => "守夜人查看同伴";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.FindBuddy };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add($"查看其他守夜人", new Choice(table.Round.Phase, table.Round.Name, $"查看其他守夜人", new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "FindBuddy", Role.Mason.ToString() }
            }));
            return choices;
        }
    }
}
