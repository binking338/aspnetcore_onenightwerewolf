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
                choices.Add($"{grave.No}号中间牌", new Choice(table.Round.Phase, table.Round.Name, $"{grave.No}号中间牌", new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "SwapWithGraveCard", $"{grave.No}" }
                }));
            }
            return choices;
        }
    }
}
