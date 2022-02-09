using System;
using System.Collections.Generic;
using System.Linq;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Werewolf
{
    public class WerewolfFindBuddyOption : IOption
    {
        public string Name => "狼人查看同伴";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.FindBuddy };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            if(table.Seats.Count(s => s.OriginCard.Role == Role.Werewolf) > 1)
            {
                choices.Add($"查看其他狼人", new Choice(table.Round.Phase, table.Round.Name, $"查看其他狼人", new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "FindBuddy", Role.Werewolf.ToString() }
                }));
            }
            return choices;
        }
    }


    public class WerewolfSeeGraveCardOption : IOption
    {
        public string Name => "狼人查看中间牌堆";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SeeGraveCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();

            if (table.Seats.Count(s => s.OriginCard.Role == Role.Werewolf) == 1)
            {
                foreach (var grave in table.Graves)
                {
                    choices.Add($"{grave.No}号中间牌", new Choice(table.Round.Phase, table.Round.Name, $"{grave.No}号中间牌", new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SeeGraveCard", $"{grave.No}" }
                    }));
                }
            }
            return choices;
        }
    }
}
