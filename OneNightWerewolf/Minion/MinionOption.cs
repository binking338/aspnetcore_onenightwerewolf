using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Minion
{
    public class MinionOption : IOption
    {
        public string Name => "爪牙查看狼人";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.FindBuddy };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add($"查看狼人", new Choice(table.Round.Phase, table.Round.Name, $"查看狼人", new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "FindBuddy", Role.Werewolf.ToString() }
            }));
            return choices;
        }
    }
}
