using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;
using OneNightWerewolf.Doppelganger;
using Action = OneNightWerewolf.Core.Action;

namespace OneNightWerewolf.Common
{
    public static class ActionHandlers
    {
        public static IActionHandler[] All
        {
            get
            {
                return new IActionHandler[]
                {
                    new NoneActionHandler(),
                    new FindBuddyActionHandler(),
                    new SeeGraveCardActionHandler(),
                    new SeeOthersCardActionHandler(),
                    new SeeMyCardActionHandler(),
                    new SwapOthersActionHandler(),
                    new SwapWithOthersActionHandler(),
                    new SwapWithGraveCardActionHandler(),
                    new CopyOthersCardActionHandler(),
                    new VoteActionHandler(),
                    new TicketsActionHandler(),
                    new HuntActionHandler(),
                    new JudgeActionHandler(),
                    new LeaveActionHandler(),
                    new ReadyActionHandler(),
                };
            }
        }
    }

    public class NoneActionHandler : IActionHandler
    {
        public Action Action => Action.None;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            // do nothing;
        }
    }

    public class FindBuddyActionHandler : IActionHandler
    {
        public Action Action => Action.FindBuddy;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            foreach (var strRole in choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var role = (Role)Enum.Parse(typeof(Role), strRole);
                var roleName = "";
                switch (role)
                {
                    case Role.Werewolf:
                        roleName = "狼人";
                        break;
                    case Role.Mason:
                        roleName = "守夜人";
                        break;
                }
                table.FindRole(seat.Player, role, roleName);
            }
        }
    }

    public class SeeGraveCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeGraveCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var graves = choice.Data["SeeGraveCard"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var grave in graves)
            {
                table.SeeCardInGrave(seat.Player, grave);
            }
        }
    }

    public class SeeOthersCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeOthersCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var players = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var player in players)
            {
                table.SeeCardInSeat(player, seat.Player);
            }
        }
    }

    public class SeeMyCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeMyCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.SeeMyCard(seat.Player);
        }
    }

    public class SwapOthersActionHandler : IActionHandler
    {
        public Action Action => Action.SwapOthers;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var players = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            table.SwapCardsBetweenSeats(players[0], players[0], players[1]);
        }
    }

    public class SwapWithOthersActionHandler : IActionHandler
    {
        public Action Action => Action.SwapWithOthers;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var player = choice.Data[Action.ToString()];

            table.SwapCardsBetweenSeats(seat.Player, seat.Player, player);
        }
    }

    public class SwapWithGraveCardActionHandler : IActionHandler
    {
        public Action Action => Action.SwapWithGraveCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var player = choice.Data[Action.ToString()];

            table.SwapCardsBetweenSeatAndGrave(seat.Player, seat.Player, player);
        }
    }

    public class CopyOthersCardActionHandler : IActionHandler
    {
        public Action Action => Action.CopyOthersCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var player = choice.Data[Action.ToString()];
            var copyFromSeat = table.FindSeat(player);
            if(seat.OriginCard is DoppelgangerCard doppelgangerCard)
            {
                doppelgangerCard.Copy(copyFromSeat.FinalCard);
            }
        }
    }

    public class VoteActionHandler : IActionHandler
    {
        public Action Action => Action.Vote;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var players = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var player in players)
            {
                table.Vote(seat.Player, player);
            }
        }
    }

    public class TicketsActionHandler : IActionHandler
    {
        public Action Action => Action.Tickets;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            seat.SeeDeath(table);
        }
    }

    public class HuntActionHandler : IActionHandler
    {
        public Action Action => Action.Hunt;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var players = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var player in players)
            {
                table.Hunt(seat.Player, player);
            }
        }
    }

    public class JudgeActionHandler : IActionHandler
    {
        public Action Action => Action.Judge;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            seat.JudgeWinning(table);
        }
    }

    public class LeaveActionHandler : IActionHandler
    {
        public Action Action => Action.Leave;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.Disseat(seat.Player);
        }
    }

    public class ReadyActionHandler : IActionHandler
    {
        public Action Action => Action.Ready;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            // todo 准备机制
        }
    }
}
