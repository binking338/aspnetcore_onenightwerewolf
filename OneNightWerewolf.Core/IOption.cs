using System;
using System.Collections.Generic;

namespace OneNightWerewolf.Core
{
    public interface IOption
    {
        public string Name { get; }

        public Action[] Actions { get; }

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table);
    }

    public class NoneOption : IOption
    {
        public string Name => "无操作";

        public Action[] Actions => new Action[] { Action.None };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices[Name] = new Choice(table.Round.Phase, table.Round.Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "None", string.Empty }
            });
            return choices;
        }
    }
}
