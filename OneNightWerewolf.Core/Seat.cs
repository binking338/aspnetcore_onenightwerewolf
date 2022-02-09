using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Seat : ICardPlace
    {
        public Seat(string no)
        {
            No = no;
            Monitor = new HtmlMonitor();
        }

        public string No { get; }

        public string Player { get; private set; }

        public IMonitor Monitor { get; set; }

        public Seat TicketVotedFor { get; private set; }

        public int TicketsReceived { get; private set; }

        public bool Dead { get; private set; }

        public bool Win { get; set; }

        public ICard OriginCard { get; set; }

        public ICard FinalCard { get; set; }

        void ICardPlace.RecycleCard()
        {
            (this as ICardPlace).OriginCard = null;
            (this as ICardPlace).FinalCard = null;
            Monitor.Clear();
            TicketVotedFor = null;
            TicketsReceived = 0;
            Dead = false;
        }

        public void TakeBy(string player)
        {
            Player = player;
        }

        public void TakeOff()
        {
            Player = string.Empty;
        }

        public void SeeCard(IMonitor monitor)
        {
            if(monitor == Monitor)
            {
                monitor.Print(string.Format(Constants.MONITOR_SEE_MY_CARD, Player, FinalCard.Name));
            }
            else
            {
                monitor.Print(string.Format(Constants.MONITOR_SEE_OTHERS_CARD, Player, FinalCard.Name));
            }
        }

        public void SeeDeath(Table table)
        {
            table.Tickets();
            if (Dead)
            {
                Monitor.Print(string.Format(Constants.MONITOR_DEAD, this.FinalCard.Name, this.TicketsReceived));
            }
            else
            {
                Monitor.Print(string.Format(Constants.MONITOR_SURVIVOR, this.FinalCard.Name, this.TicketsReceived));
            }
        }

        public void Vote(Seat seat)
        {
            TicketVotedFor = seat;
            seat.TicketsReceived++;
        }

        public void Die()
        {
            Dead = true;
        }

        public void JudgeWinning(Table table)
        {
            table.JudgeWinningCamp();
            this.Win = FinalCard.JudgeWinning(table, this.No);
            if (Win)
            {
                Monitor.Print(Constants.MONITOR_WINNER);
            }
            else
            {
                Monitor.Print(Constants.MONITOR_LOSER);
            }
        }
    }
}
