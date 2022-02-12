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
                table.FindRole(seat.No, role, roleName);
            }
        }
    }

    public class SeeGraveCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeGraveCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetGraveNos = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var targetGraveNo in targetGraveNos)
            {
                table.SeeCardInGrave(seat.No, targetGraveNo);
            }
        }
    }

    public class SeeOthersCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeOthersCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetSeatNos = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var targetSeatNo in targetSeatNos)
            {
                table.SeeCardInSeat(seat.No, targetSeatNo);
            }
        }
    }

    public class SeeMyCardActionHandler : IActionHandler
    {
        public Action Action => Action.SeeMyCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.SeeMyCard(seat.No);
        }
    }

    public class SwapOthersActionHandler : IActionHandler
    {
        public Action Action => Action.SwapOthers;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetSeatNos = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            table.SwapCardsBetweenSeats(seat.No, targetSeatNos[0], targetSeatNos[1]);
        }
    }

    public class SwapWithOthersActionHandler : IActionHandler
    {
        public Action Action => Action.SwapWithOthers;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetSeatNo = choice.Data[Action.ToString()];
            table.SwapCardsBetweenSeats(seat.No, seat.No, targetSeatNo);
        }
    }

    public class SwapWithGraveCardActionHandler : IActionHandler
    {
        public Action Action => Action.SwapWithGraveCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetGraveNo = choice.Data[Action.ToString()];
            table.SwapCardsBetweenSeatAndGrave(seat.No, seat.No, targetGraveNo);
        }
    }

    public class CopyOthersCardActionHandler : IActionHandler
    {
        public Action Action => Action.CopyOthersCard;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var copyFromSeatNo = choice.Data[Action.ToString()];
            var copyFromSeat = table.FindSeat(copyFromSeatNo);
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
            var targetSeatNos = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var targetSeatNo in targetSeatNos)
            {
                table.Vote(seat.No, targetSeatNo);
            }
        }
    }

    public class TicketsActionHandler : IActionHandler
    {
        public Action Action => Action.Tickets;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.SeeDeath(seat.No);
        }
    }

    public class HuntActionHandler : IActionHandler
    {
        public Action Action => Action.Hunt;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            if (!choice.Data.ContainsKey(Action.ToString()) || string.IsNullOrWhiteSpace(choice.Data[Action.ToString()])) return;
            var targetSeatNos = choice.Data[Action.ToString()].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var targetSeatNo in targetSeatNos)
            {
                table.Hunt(seat.No, targetSeatNo);
            }
        }
    }

    public class JudgeActionHandler : IActionHandler
    {
        public Action Action => Action.Judge;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.JudgeWinning(seat.No);
        }
    }

    public class ReadyActionHandler : IActionHandler
    {
        public Action Action => Action.Ready;

        public void Handle(Table table, Seat seat, Choice choice)
        {
            table.Ready(seat.No);
        }
    }
}
