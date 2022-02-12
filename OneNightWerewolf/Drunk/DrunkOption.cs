using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Drunk
{
    public class DrunkOption : IOption
    {
        public string Name => "酒鬼替换身份";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SwapWithGraveCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            foreach (var grave in table.Graves)
            {
                var choice = $"交换{grave.No}号牌";
                choices.Add(choice, new Choice(table.Round.Phase, table.Round.Name, choice, new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "SwapWithGraveCard", $"{grave.No}" }
                }));
            }
            return choices;
        }
    }
}
