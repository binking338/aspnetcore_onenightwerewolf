﻿using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Robber
{
    public class RobberOption : IOption
    {
        public string Name => "强盗交换身份";

        public Core.Action[] Actions => new Core.Action[] { Core.Action.SwapWithOthers, Core.Action.SeeMyCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            for (var i = 0; i < table.GetSeats().Length; i++)
            {
                if (seat.No == table.GetSeats()[i].No) continue;
                var choice = $"交换[{table.GetSeats()[i].Player}]";
                choices.Add(choice, new Choice(table.GetRound().Phase, table.GetRound().Name, choice, new Dictionary<string, string>() {
                        { "Option", $"{Name}" },
                        { "SwapWithOthers", $"{table.GetSeats()[i].No}"},
                        { "SeeMyCard", $"{seat.No}"}
                    }));
            }
            return choices;
        }
    }
}
