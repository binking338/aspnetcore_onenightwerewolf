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
            var choice = $"查看其他守夜人";
            choices.Add(choice, new Choice(table.Round.Phase, table.Round.Name, choice, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "FindBuddy", Role.Mason.ToString() }
            }));
            return choices;
        }
    }
}
