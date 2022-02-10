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
            TicketsReceivedFrom = new List<Seat>();
        }

        public string No { get; }

        public string Player { get; private set; }

        public IMonitor Monitor { get; set; }

        public Seat TicketVotedFor { get; private set; }

        public IList<Seat> TicketsReceivedFrom { get; private set; }

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
            TicketsReceivedFrom.Clear();
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

        public void Vote(Seat seat)
        {
            this.TicketVotedFor = seat;
            seat.BeVoted(this);
        }

        public void BeVoted(Seat seat)
        {
            this.TicketsReceived++;
        }

        public void Die()
        {
            Dead = true;
        }
    }
}
