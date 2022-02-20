using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;
using Action = OneNightWerewolf.Core.Action;

namespace OneNightWerewolf.Common
{
    public class StartOption : IOption
    {
        public string Name => "查看自己的角色";

        public Action[] Actions => new Action[] { Action.SeeMyCard };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices[Name] = new Choice(table.GetRound().Phase, table.GetRound().Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "SeeMyCard", seat.No }
            });
            return choices;
        }
    }

    public class VoteOption : IOption
    {
        public string Name => "投票";

        public Action[] Actions => new Action[] { Action.Vote };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            foreach (var s in table.GetSeats())
            {
                if (s.No == seat.No) continue;
                var choice = $"[{s.Player}]是狼人";
                choices.Add(choice, new Choice(table.GetRound().Phase, table.GetRound().Name, choice, new Dictionary<string, string>() {
                    { "Option", $"{Name}" },
                    { "Vote", s.No }
                }));
            }
            return choices;
        }
    }

    public class TicketsOption : IOption
    {
        public string Name => "投票结果";

        public Action[] Actions => new Action[] { Action.Tickets };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add(Name, new Choice(table.GetRound().Phase, table.GetRound().Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "Tickets", $"{seat.No}" }
            }));
            return choices;
        }
    }

    public class JudgeOption : IOption
    {
        public string Name => "游戏结局";

        public Action[] Actions => new Action[] { Action.Judge };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add(Name, new Choice(table.GetRound().Phase, table.GetRound().Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "Judge",  $"{seat.No}" }
            }));
            return choices;
        }
    }

    public class ReadyOption : IOption
    {
        public string Name => "准备";

        public Action[] Actions => new Action[] { Action.Ready };

        public IDictionary<string, Choice> GenerateChoices(Seat seat, Table table)
        {
            var choices = new Dictionary<string, Choice>();
            choices.Add(Name, new Choice(table.GetRound().Phase, table.GetRound().Name, Name, new Dictionary<string, string>() {
                { "Option", $"{Name}" },
                { "Ready", seat.No }
            }));
            return choices;
        }
    }
}
