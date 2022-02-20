using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Table
    {
        public Table(Game game)
        {
            Game = game;
        }

        public Game Game { get; private set; }

        public Seat[] Seats { get; private set; }

        public Grave[] Graves { get; private set; }

        public DateTime? StartTime { get; private set; }

        public DateTime? DawnTime { get; private set; }

        public DateTime? OverTime { get; private set; }

        public int RoundIndex { get; private set; } = -1;

        public Camp? WinningCamp { get; private set; }

        public Dictionary<string, Choice> SeatChoices { get; } = new Dictionary<string, Choice>();

        public GameReplay Replay { get; } = new GameReplay();

        public DefaultMonitor Monitor { get; set; } = new DefaultMonitor();

        #region Game

        public void Config(int seats, int graves)
        {
            Seats = new Seat[seats];
            for (int i = 0; i < seats; i++)
            {
                Seats[i] = new Seat((i + 1).ToString());
            }
            Graves = new Grave[graves];
            for (int i = 0; i < graves; i++)
            {
                Graves[i] = new Grave((i + 1).ToString());
            }
        }

        public bool NewGame()
        {
            RoundIndex = -1;
            StartTime = null;
            DawnTime = null;
            OverTime = null;

            SeatChoices.Clear();
            Replay.Histories.Clear();

            Monitor.Clear();
            WinningCamp = null;

            return true;
        }

        public IRound GetRound()
        {
            return Game.Rounds[RoundIndex];
        }

        public bool IsGameFinished()
        {
            return GetRound() == null || IsLastRound();
        }

        public void Recycle()
        {
            foreach (var seat in Seats)
            {
                (seat as ICardPlace).RecycleCard();
            }
        }

        public void Deal(ICard[] cards)
        {
            if (cards.Length != Seats.Length + Graves.Length)
            {
                throw new ArgumentException("牌不够");
            }
            var i = 0;
            foreach (var seat in Seats)
            {
                (seat as ICardPlace).PutCard(cards[i++]);
            }
            foreach (var grave in Graves)
            {
                (grave as ICardPlace).PutCard(cards[i++]);
            }
        }

        public void Tickets()
        {
            if (this.Seats.All(s => s.TicketsReceived == 1))
            {
                // no one die
            }
            else
            {
                var max = this.Seats.Select(s => s.TicketsReceived).Max();
                this.Seats.Where(s => s.TicketsReceived == max).ToList()
                    .ForEach(s => s.Die());
            }
        }

        public void JudgeWinningCamp()
        {
            if (WinningCamp != null) return;
            WinningCamp = Game.WinningCampDecisionRule.Judge(this);
            switch (WinningCamp)
            {
                case Camp.None:
                    Monitor.Print(DefaultMonitor.Of(Constants.MONITOR_WINNING_CAMP_NONE, this));
                    break;
                case Camp.Villiage:
                    Monitor.Print(DefaultMonitor.Of(Constants.MONITOR_WINNING_CAMP_VILLIAGE, this));
                    break;
                case Camp.Werewolf:
                    Monitor.Print(DefaultMonitor.Of(Constants.MONITOR_WINNING_CAMP_WEREWOLF, this));
                    break;
            }
        }

        public void NextRound()
        {
            if(GetRound() == null && RoundIndex == -1)
            {
                RoundIndex = 0;
                StartTime = DateTime.Now;
            }
            if (!IsRoundFinished() || IsLastRound())
            {
                return;
            }

            Replay.Histories.Add(new GameRoundHistory(GetRound(), new Dictionary<string, Choice>(SeatChoices)));
            SeatChoices.Clear();
            do
            {
                RoundIndex++;

            } while (!GetRound().Enabled(this));
            if(GetRound().Phase >= Phase.Dawn && !DawnTime.HasValue)
            {
                DawnTime = DateTime.Now;
            }
            else if(GetRound().Phase == Phase.Over && !OverTime.HasValue)
            {
                OverTime = DateTime.Now;
                foreach(var seat in Seats)
                {
                    seat.Ready = false;
                }
            }
        }

        public bool IsRoundFinished()
        {
            if (SeatChoices.Count == Seats.Length)
            {
                return true;
            }
            return false;
        }

        public bool IsLastRound()
        {
            if (Game.Rounds.Length == RoundIndex + 1)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Player

        public bool IsAllSeatTaken()
        {
            if (Seats == null) return false;
            foreach (var seat in Seats)
            {
                if (string.IsNullOrWhiteSpace(seat.Player))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsAllSeatReady()
        {
            if (Seats == null) return false;
            foreach (var seat in Seats)
            {
                if (string.IsNullOrWhiteSpace(seat.Player) || !seat.Ready)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Takeseat(string player)
        {
            if (IsAllSeatTaken())
            {
                return false;
            }
            foreach (var seat in Seats)
            {
                if (string.IsNullOrWhiteSpace(seat.Player))
                {
                    seat.TakeBy(player);
                    return true;
                }
            }
            return false;
        }

        public void Disseat(string player)
        {
            var seat = FindSeatByNick(player);
            seat.TakeOff();
        }

        public void Ready(string seatNo)
        {
            var seat = FindSeat(seatNo);
            seat.Ready = true;
        }

        #endregion

        #region Card Oprations

        public IDictionary<string, Choice> GetChoices(string seatNo)
        {
            var seat = FindSeat(seatNo);
            var choices = new Dictionary<string, Choice>();
            var ability = GetAbility(seat.No);
            if (ability == null) return null;
            foreach (var option in ability.Options)
            {
                foreach (var choice in option.GenerateChoices(seat, this))
                {
                    choices.Add(choice.Key, choice.Value);
                }
            }
            return choices;
        }

        public bool IsChoiceMade(string seatNo)
        {
            if (SeatChoices.ContainsKey(seatNo))
            {
                return true;
            }
            return false;
        }

        public bool MakeChoice(string seatNo, Choice choice)
        {
            if (IsChoiceMade(seatNo))
            {
                return false;
            }
            SeatChoices.Add(seatNo, choice);
            var seat = FindSeat(seatNo);
            var ability = GetAbility(seat.No);
            var option = ability.Options?.FirstOrDefault(option => option.Name == choice.Data["Option"]);
            var actions = option == null ? new Action[] { Action.None } : option.Actions;
            if (Game.ActionHandlers != null)
            {
                foreach (var action in actions)
                {
                    foreach (var actionHandler in Game.ActionHandlers)
                    {
                        if (action == actionHandler.Action) actionHandler.Handle(this, seat, choice);
                    }
                }
            }
            return true;
        }

        private IAbility GetAbility(string seatNo)
        {
            var seat = FindSeat(seatNo);
            if (GetRound() == null) return null;
            var card = GetRound().Phase > Phase.Dawn ? seat.FinalCard : seat.OriginCard;
            foreach (var ability in card.Abilities)
            {
                if (ability.TriggerCondition?.Invoke(GetRound()) == true)
                {
                    return ability;
                }
            }
            return new NoneAbility();
        }

        public void FindRole(string seatNo, Role role, string roleName)
        {
            var seat = FindSeat(seatNo);

            List<Seat> roleSeats = new List<Seat>();
            foreach (var s in Seats)
            {
                if (s.No == seat.No)
                {
                    continue;
                }
                if (s.OriginCard.Role == role)
                {
                    roleSeats.Add(s);
                }
            }
            if (roleSeats.Count == 0)
            {
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_NONE_PLAYER, roleName), this));
            }
            else
            {
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_FIND_ROLE_PLAYER, roleName, string.Join(",", roleSeats.Select(w => w.Player))), this));
            }
        }

        public void SeeMyCard(string seatNo)
        {
            var seat = FindSeat(seatNo);

            seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SEE_MY_CARD, seat.Player, seat.FinalCard.Name), this));
        }

        public void SeeCardInSeat(string seatNo, string targetSeatNo)
        {
            var seat = FindSeat(seatNo);
            var target = FindSeat(targetSeatNo);

            seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SEE_OTHERS_CARD, target, target.FinalCard.Name), this));
        }

        public void SeeCardInGrave(string seatNo, string targetGraveNo)
        {
            var seat = FindSeat(seatNo);
            var grave = FindGrave(targetGraveNo);
            seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SEE_GRAVE_CARD, grave.No, grave.FinalCard.Name), this));
        }

        public void SwapCardsBetweenSeats(string seatNo, string targetSeatNo1, string targetSeatNo2)
        {
            var seat = FindSeat(seatNo);
            var seat1 = FindSeat(targetSeatNo1);
            var seat2 = FindSeat(targetSeatNo2);
            (seat1 as ICardPlace).SwapCardWith(seat2);
            if (seatNo == targetSeatNo1)
            {
                // 自己与他人交换
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SWAP_WITH_OTHERS, seat2.Player), this));
            }
            else if (seatNo == targetSeatNo2)
            {
                // 自己与他人交换
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SWAP_WITH_OTHERS, seat1.Player), this));
            }
            else
            {
                // 交换其他人
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SWAP_OTHERS, seat1.Player, seat2.Player), this));
            }
        }

        public void SwapCardsBetweenSeatAndGrave(string seatNo, string targetSeatNo, string targetGraveNo)
        {
            var seat = FindSeat(seatNo);
            var seat1 = FindSeat(targetSeatNo);
            var grave = FindGrave(targetGraveNo);
            (seat1 as ICardPlace).SwapCardWith(grave);
            if (seatNo == targetSeatNo)
            {
                // 自己与中间牌交换
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SWAP_WITH_GRAVE, targetGraveNo), this));
            }
            else
            {
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SWAP_OHTERS_WITH_GRAVE, seat1.No, targetGraveNo), this));
            }
        }

        public void Vote(string voterSeatNo, string beVotedSeatNo)
        {
            var seat = FindSeat(voterSeatNo);
            var votedSeat = FindSeat(beVotedSeatNo);
            seat.Vote(votedSeat);
        }

        public void SeeDeath(string seatNo)
        {
            this.Tickets();
            var seat = FindSeat(seatNo);
            if (seat.Dead)
            {
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_DEAD, seat.FinalCard.Name, seat.TicketsReceived), this));
            }
            else
            {
                seat.Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_SURVIVOR, seat.FinalCard.Name, seat.TicketsReceived), this));
            }
        }

        public void Hunt(string hunterSeatNo, string beHuntedSeatNo)
        {
            var hunterSeat = FindSeat(hunterSeatNo);
            var beHuntedSeat = FindSeat(beHuntedSeatNo);
            beHuntedSeat.Die();
            Monitor.Print(DefaultMonitor.Of(string.Format(Constants.MONITOR_HUNT, hunterSeat.No, beHuntedSeat.Player), this));
        }


        public void JudgeWinning(string seatNo)
        {
            this.JudgeWinningCamp();
            var seat = FindSeat(seatNo);
            seat.Win = seat.FinalCard.JudgeWinning(this, seat.No);
            if (seat.Win)
            {
                seat.Monitor.Print(DefaultMonitor.Of(Constants.MONITOR_WINNER, this));
            }
            else
            {
                seat.Monitor.Print(DefaultMonitor.Of(Constants.MONITOR_LOSER, this));
            }
        }

        public Seat FindSeatByNick(string player)
        {
            foreach (var seat in Seats)
            {
                if (string.CompareOrdinal(player, seat.Player) == 0)
                {
                    return seat;
                }
            }
            throw new IndexOutOfRangeException($"不存在{player}玩家");
        }

        public Seat FindSeat(string no)
        {
            foreach (var seat in Seats)
            {
                if (string.CompareOrdinal(no, seat.No) == 0)
                {
                    return seat;
                }
            }
            throw new IndexOutOfRangeException($"不存在{no}号座位");
        }

        public Grave FindGrave(string no)
        {
            foreach (var grave in Graves)
            {
                if(string.CompareOrdinal(no, grave.No) == 0)
                {
                    return grave;
                }
            }
            throw new IndexOutOfRangeException($"不存在{no}号中间牌");
        }

        #endregion
    }
}
