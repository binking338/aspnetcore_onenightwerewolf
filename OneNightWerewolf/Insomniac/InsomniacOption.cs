using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;
using Action = OneNightWerewolf.Core.Action;

namespace OneNightWerewolf.Insomniac
{
    public class InsomniacOption : IOption
    {
        public string Name => "失眠者查看手牌";

        public Core.Action[] Actions => new Action[] { Action.SeeMyCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add(Name, new Choice(table.Round.Phase, table.Round.Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "SeeMyCard", seat.Player }
            }));
            return choices;
        }
    }
}
