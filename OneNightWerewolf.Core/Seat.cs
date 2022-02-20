using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Seat : ICardPlace
    {
        public Table _table;

        public Seat(string no)
        {
            No = no;
            Monitor = new DefaultMonitor();
            TicketsReceivedFrom = new List<string>();
        }

        public string OriginCardId { get; set; }

        public string FinalCardId { get; set; }

        public string No { get; set; }

        public string Player { get; set; }

        public bool Ready { get; set; }

        public DefaultMonitor Monitor { get; set; }

        public string TicketVotedFor { get; set; }

        public List<string> TicketsReceivedFrom { get; set; }

        public int TicketsReceived { get; set; }

        public bool Dead { get; set; }

        public bool Win { get; set; }

        public void Clone(Seat seat)
        {
            this.OriginCardId = seat.OriginCardId;
            this.FinalCardId = seat.FinalCardId;
            this.No = seat.No;
            this.Player = seat.Player;
            this.Ready = seat.Ready;
            this.Monitor = seat.Monitor;
            this.TicketVotedFor = seat.TicketVotedFor;
            this.TicketsReceivedFrom = seat.TicketsReceivedFrom;
            this.TicketsReceived = seat.TicketsReceived;
            this.Dead = seat.Dead;
            this.Win = seat.Win;
        }

        public Seat Assosiate(Table table)
        {
            _table = table;
            return this;
        }

        public ICard GetOriginCard()
        {
            return _table.GetGame().Cards.First(c => c.Id == OriginCardId);
        }

        public ICard GetFinalCard()
        {
            return _table.GetGame().Cards.First(c => c.Id == FinalCardId);
        }

        public virtual void SwapCardWith(ICardPlace cardPlace)
        {
            var cardId = FinalCardId;
            FinalCardId = cardPlace.FinalCardId;
            cardPlace.FinalCardId = cardId;
        }

        public virtual void PutCard(ICard card)
        {
            OriginCardId = card.Id;
            FinalCardId = card.Id;
        }

        void ICardPlace.RecycleCard()
        {
            OriginCardId = null;
            FinalCardId = null;
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
