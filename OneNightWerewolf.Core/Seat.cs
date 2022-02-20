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
            Monitor = new DefaultMonitor();
            TicketsReceivedFrom = new List<string>();
        }

        public string No { get; private set; }

        public string Player { get; private set; }

        public bool Ready { get; set; }

        public DefaultMonitor Monitor { get; set; }

        public string TicketVotedFor { get; private set; }

        public List<string> TicketsReceivedFrom { get; private set; }

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
            Ready = true;
        }

        public void TakeOff()
        {
            Player = string.Empty;
            Ready = false;
        }

        public void Vote(Seat seat)
        {
            this.TicketVotedFor = seat.No;
            seat.BeVoted(this);
        }

        public void BeVoted(Seat seat)
        {
            this.TicketsReceived++;
            this.TicketsReceivedFrom.Add(seat.No);
        }

        public void Die()
        {
            Dead = true;
        }
    }
}
