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

        public IMonitor Monitor { get; private set; }

        public Seat SeatVoted { get; private set; }

        public int Tickets { get; private set; }

        public bool Dead { get; private set; }

        public bool Win { get; set; }

        public ICard OriginCard { get; set; }

        public ICard FinalCard { get; set; }

        void ICardPlace.RecycleCard()
        {
            (this as ICardPlace).OriginCard = null;
            (this as ICardPlace).FinalCard = null;
            Monitor.Clear();
            SeatVoted = null;
            Tickets = 0;
            Dead = false;
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
            SeatVoted = seat;
            seat.Tickets++;
        }

        public void Die()
        {
            Dead = true;
        }

        public void FindRole(IList<Seat> seats, Role role, string roleName)
        {
            List<Seat> werewolfs = new List<Seat>();
            foreach (var seat in seats)
            {
                if (seat.No == this.No)
                {
                    continue;
                }
                if (seat.OriginCard.Role == role)
                {
                    werewolfs.Add(seat);
                }
            }
            if (werewolfs.Count == 0)
            {
                this.Monitor.Print(string.Format(Constants.MONITOR_NONE_PLAYER, roleName));
            }
            else
            {
                this.Monitor.Print(string.Format(Constants.MONITOR_FIND_ROLE_PLAYER, roleName, string.Join(",", werewolfs.Select(w => w.Player))));
            }
        }

        public void CountTickets(Table table)
        {
            table.Tickets();
            if (Dead)
            {
                Monitor.Print(string.Format(Constants.MONITOR_DEAD, this.FinalCard.Name,this.Tickets));
            }
            else
            {
                Monitor.Print(string.Format(Constants.MONITOR_SURVIVOR, this.FinalCard.Name, this.Tickets));
            }
        }

        public void Judge(Table table)
        {
            table.Judge();
            switch (this.FinalCard.Role)
            {
                case Role.Tanner:
                    this.Win = this.Dead;
                    break;
                case Role.Werewolf:
                case Role.Minion:
                    this.Win = table.WinCamp == Camp.Werewolf;
                    break;
                default:
                    this.Win = table.WinCamp == Camp.Villiage;
                    break;
            }
            if (Win)
            {
                Monitor.Print(Constants.MONITOR_WINNER);
            }
            else
            {
                Monitor.Print(Constants.MONITOR_LOSER);
            }
        }

        public void Set(IMonitor monitor)
        {
            Monitor = monitor;
        }
    }
}
