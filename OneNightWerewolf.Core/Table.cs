using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    // todo 胜负阵营判定抽象出来 WinningCampJudgeRule 判定上下文 table
    // todo 卡牌胜负判定规则抽象出来 WinnerJudgeRule 判定上下文 table seat.no
    public class Table
    {
        public Table(Game game)
        {
            Game = game;
        }

        public Game Game { get; private set; }

        public Grave[] Graves { get; private set; }

        public Seat[] Seats { get; private set; }

        public DateTime? StartTime { get; private set; }

        public DateTime? DawnTime { get; private set; }

        public DateTime? OverTime { get; private set; }

        public int RoundIndex { get; private set; } = -1;

        public IRound Round { get; private set; } = null;

        public IDictionary<string, Choice> PlayerChoices { get; } = new Dictionary<string, Choice>();

        public GameReplay Replay { get; } = new GameReplay();

        public IMonitor Monitor { get; set; } = new HtmlMonitor();

        public Camp? WinningCamp { get; private set; }

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
            Round = null;
            StartTime = null;
            DawnTime = null;
            OverTime = null;

            PlayerChoices.Clear();
            Replay.Histories.Clear();

            Monitor.Clear();
            WinningCamp = null;

            return true;
        }

        public bool IsGameFinished()
        {
            return Round == null || IsLastRound();
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

        public void NextRound()
        {
            if(Round == null && RoundIndex == -1)
            {
                RoundIndex = 0;
                Round = Game.Rounds[0];
                StartTime = DateTime.Now;
            }
            if (!IsRoundFinished() || IsLastRound())
            {
                return;
            }

            Replay.Histories.Add(new GameRoundHistory(Round, new Dictionary<string, Choice>(PlayerChoices)));
            PlayerChoices.Clear();
            do
            {
                RoundIndex++;
                Round = Game.Rounds[RoundIndex];

            } while (!Round.Enabled(this));
            if(Round.Phase >= Phase.Dawn && !DawnTime.HasValue)
            {
                DawnTime = DateTime.Now;
            }
            else if(Round.Phase == Phase.Over && !OverTime.HasValue)
            {
                OverTime = DateTime.Now;
            }
        }

        public bool IsRoundFinished()
        {
            if (PlayerChoices.Count == Seats.Length)
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
            var seat = FindSeat(player);
            seat.TakeOff();
        }

        #endregion

        #region Card Oprations

        public IDictionary<string, Choice> GetChoices(string player)
        {
            var seat = FindSeat(player);
            var choices = new Dictionary<string, Choice>();
            var ability = GetAbility(player);
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

        public bool IsChoiceMade(string player)
        {
            if (PlayerChoices.ContainsKey(player))
            {
                return true;
            }
            return false;
        }

        public bool MakeChoice(string player, Choice choice)
        {
            if (IsChoiceMade(player))
            {
                return false;
            }
            PlayerChoices.Add(player, choice);
            var ability = GetAbility(player);
            var option = ability.Options?.FirstOrDefault(option => option.Name == choice.Data["Option"]);
            var actions = option == null ? new Action[] { Action.None } : option.Actions;
            if (Game.ActionHandlers != null)
            {
                var seat = FindSeat(player);
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

        private IAbility GetAbility(string player)
        {
            var seat = FindSeat(player);
            if (Round == null) return null;
            var card = Round.Phase > Phase.Dawn ? seat.FinalCard : seat.OriginCard;
            foreach (var ability in card.Abilities)
            {
                if (ability.TriggerCondition?.Invoke(Round) == true)
                {
                    return ability;
                }
            }
            return new NoneAbility();
        }

        public void FindRole(string player, Role role, string roleName)
        {
            var seat = FindSeat(player);

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
                seat.Monitor.Print(string.Format(Constants.MONITOR_NONE_PLAYER, roleName));
            }
            else
            {
                seat.Monitor.Print(string.Format(Constants.MONITOR_FIND_ROLE_PLAYER, roleName, string.Join(",", roleSeats.Select(w => w.Player))));
            }
        }

        public void SeeMyCard(string player)
        {
            var seat = FindSeat(player);
            seat.SeeCard(seat.Monitor);
        }

        public void SeeCardInSeat(string player, string targetPlayer)
        {
            var seat = FindSeat(player);
            var target = FindSeat(targetPlayer);
            seat.SeeCard(target.Monitor);
        }

        public void SeeCardInGrave(string player, string graveNo)
        {
            var seat = FindSeat(player);
            var grave = FindGrave(graveNo);
            grave.SeeCard(seat.Monitor);
        }

        public void SwapCardsBetweenSeats(string player, string player1, string player2)
        {
            var seat = FindSeat(player);
            var seat1 = FindSeat(player1);
            var seat2 = FindSeat(player2);
            (seat1 as ICardPlace).SwapCardWith(seat2);
            if (player == player1)
            {
                // 自己与他人交换
                seat.Monitor.Print(string.Format(Constants.MONITOR_SWAP_WITH_OTHERS,player2));
            }
            else if (player == player2)
            {
                // 自己与他人交换
                seat.Monitor.Print(string.Format(Constants.MONITOR_SWAP_WITH_OTHERS, player1));
            }
            else
            {
                // 交换其他人
                seat.Monitor.Print(string.Format(Constants.MONITOR_SWAP_OTHERS, player1, player2));
            }
        }

        public void SwapCardsBetweenSeatAndGrave(string player, string player1, string graveNo)
        {
            var seat = FindSeat(player);
            var seat1 = FindSeat(player1);
            var grave = FindGrave(graveNo);
            (seat1 as ICardPlace).SwapCardWith(grave);
            if (player == player1)
            {
                // 自己与中间牌交换
                seat.Monitor.Print(string.Format(Constants.MONITOR_SWAP_WITH_GRAVE, graveNo));
            }
            else
            {
                seat.Monitor.Print(string.Format(Constants.MONITOR_SWAP_OHTERS_WITH_GRAVE, player1, graveNo));
            }
        }

        public void Vote(string voter, string beVotedPlayer)
        {
            var seat = FindSeat(voter);
            var votedSeat = FindSeat(beVotedPlayer);
            seat.Vote(votedSeat);
        }

        public void Hunt(string hunter, string beHuntedPlayer)
        {
            var votedSeat = FindSeat(beHuntedPlayer);
            votedSeat.Die();
            Monitor.Print(string.Format(Constants.MONITOR_HUNT, hunter, beHuntedPlayer));
        }

        public void JudgeWinningCamp()
        {
            if (WinningCamp != null) return;
            var deaths = this.Seats.Where(s => s.Dead);
            var hasWerewolf = this.Seats.Any(s => s.FinalCard.Role == Role.Werewolf);
            var hasMinion = this.Seats.Any(s => s.FinalCard.Role == Role.Minion);
            
            if (deaths.Count() > 0 && deaths.All(s => s.FinalCard.Role == Role.Tanner))
            {
                // 只死了皮匠
                WinningCamp = Camp.None;
                return;
            }
            else if (hasWerewolf)
            {
                // 有狼人，死任一狼人
                if (deaths.Any(s => s.FinalCard.Role == Role.Werewolf))
                {
                    // 村民赢
                    WinningCamp = Camp.Villiage;
                }
                else
                {
                    // 狼人赢
                    WinningCamp = Camp.Werewolf;
                }
            }
            else if(hasMinion)
            {
                // 没狼人 有爪牙
                if (deaths.Any(s => s.FinalCard.Role == Role.Minion) && !deaths.Any(s => s.FinalCard.Role != Role.Minion && s.FinalCard.Role != Role.Tanner))
                {
                    // 村民赢
                    WinningCamp = Camp.Villiage;
                }
                else if(!deaths.Any(s => s.FinalCard.Role == Role.Minion) && deaths.Any(s => s.FinalCard.Role != Role.Minion && s.FinalCard.Role != Role.Tanner))
                {
                    // 狼人赢
                    WinningCamp = Camp.Werewolf;
                }
                else
                {
                    // 没有阵营赢
                    WinningCamp = Camp.None;
                }
            }
            else if(deaths.Count() == 0)
            {
                WinningCamp = Camp.Villiage;
            }
            else
            {
                WinningCamp = Camp.None;
            }
            switch (WinningCamp)
            {
                case Camp.None:
                    Monitor.Print(Constants.MONITOR_WINNING_CAMP_NONE);
                    break;
                case Camp.Villiage:
                    Monitor.Print(Constants.MONITOR_WINNING_CAMP_VILLIAGE);
                    break;
                case Camp.Werewolf:
                    Monitor.Print(Constants.MONITOR_WINNING_CAMP_WEREWOLF);
                    break;
            }
        }

        public Seat FindSeat(string player)
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
