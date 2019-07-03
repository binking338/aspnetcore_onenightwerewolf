using System;
using System.Linq;
using System.Collections.Generic;

namespace OneNightWerewolf
{
    public class Game
    {
        public const int CENTER_CARD_NUM = 3;
        private static Random random;
        static Game()
        {
            random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
        }

        public Game(GameRoomProvider gameRoomProvider)
        {
            this.RoomProvider = gameRoomProvider;
        }


        #region 房间

        public GameRoomProvider RoomProvider { get; set; }

        public GameRoom Room { get; set; }

        public Game SetRoomId(string roomId)
        {
            if (roomId != null && RoomProvider != null)
            {
                Room = RoomProvider.Get(roomId);
                Room.Cards.Sort(new Comparison<GameCard>((a, b) => a.Role - b.Role));
            }
            return this;
        }

        public bool ChangeCards(GameRole[] roles)
        {
            if(roles == null || roles.Length < CENTER_CARD_NUM)
            {
                return false;
            }
            if (GetCurrentPhase() != GamePhase.Over)
            {
                return false;
            }
            var cards = roles.Select(role => new GameCard() { Role = role }).OrderBy(card => card.Role).ToList();
            if (cards.Count == Room.Cards.Count)
            {
                var i = 0;
                for (; i < cards.Count; i++)
                {
                    if (cards[i].Role != Room.Cards[i].Role) break;
                }
                if(i == cards.Count)
                {
                    return true;
                }
            }
            Room.Players.ForEach(player => player.HistoryOptions.Clear());
            Room.StateStack.Clear();
            Room.Cards = cards; 
            return true;
        }

        public List<GameCard> GetAllCards()
        {
            return Room.Cards;
        }

        protected List<GamePhase> GetAllPhases()
        {
            var roles = Room.Cards.Select(card => card.Role).Distinct().OrderBy(role=>role).ToList();
            List<GamePhase> phases = new List<GamePhase>();
            phases.Add(GamePhase.Start);
            foreach (var role in roles) 
            {
                switch (role) 
                {
                    case GameRole.Village:
                    case GameRole.Hunter:
                    case GameRole.Tanner:
                        continue;
                    case GameRole.Werewolf:
                        phases.Add(GamePhase.NightWerewolf);
                        break;
                    case GameRole.Minion:
                        phases.Add(GamePhase.NightMinion);
                        break;
                    case GameRole.Masons:
                        phases.Add(GamePhase.NightMasons);
                        break;
                    case GameRole.Seer:
                        phases.Add(GamePhase.NightSeer);
                        break;
                    case GameRole.Robber:
                        phases.Add(GamePhase.NightRobber);
                        break;
                    case GameRole.Troublemaker:
                        phases.Add(GamePhase.NightTroublemaker);
                        break;
                    case GameRole.Drunk:
                        phases.Add(GamePhase.NightDrunk);
                        break;
                    case GameRole.Insomniac:
                        phases.Add(GamePhase.NightInsomniac);
                        break;
                    case GameRole.Doppelgänger:
                        phases.Add(GamePhase.NightDoppelgängerFeign);
                        phases.Add(GamePhase.NightDoppelgängerPlay);
                        if (roles.Contains(GameRole.Insomniac))
                        {
                            phases.Add(GamePhase.NightDoppelgängerInsomniac);
                        }
                        break;
                }
            }
            phases.Add(GamePhase.Day);
            phases.Sort();
            phases.Add(GamePhase.Over);
            return phases;
        }

        public bool HasEnoughPlayers()
        {
            return Room.Players.Count > 1 && (Room.Players.Count + CENTER_CARD_NUM) == Room.Cards.Count;
        }

        public bool Join(User user)
        {
            lock (Room)
            {
                if (!string.IsNullOrEmpty(user.RoomId))
                {
                    return false;
                }
                if (Room.Players.Exists(player => player.UserId == user.Id))
                {
                    return false;
                }
                if (HasEnoughPlayers())
                {
                    return false;
                }

                Room.Players.Add(new Player()
                {
                    UserId = user.Id,
                    UserNick = user.Nick
                });
                user.RoomId = Room.Id;
                for (var no = 0; no < Room.Players.Count; no++)
                {
                    Room.Players[no].SeatNo = no;
                }
            }
            return true;
        }

        public bool Leave(User user)
        {
            lock (Room)
            {
                if (string.IsNullOrEmpty(user.RoomId))
                {
                    return false;
                }
                if (!Room.Players.Exists(player => player.UserId == user.Id))
                {
                    return false;
                }
                if (user.RoomId != Room.Id)
                {
                    return false;
                }
                if (Room.Players.RemoveAll(player => player.UserId == user.Id) > 0)
                {
                    user.RoomId = null;
                    Over();
                }
                return true;
            }
        }

        public bool Kick(int seatNo)
        {
            lock(Room)
            {
                if (Room.Players.RemoveAll(player => player.SeatNo == seatNo) > 0)
                {
                    Over();
                    return true;
                }
                return false;
            }
        }

        public void Over()
        {
            Room.Players.ForEach(player => player.HistoryOptions.Clear());
            Room.Cards.ForEach(card => card.Extra = null);
            Room.StateStack.Clear();
        }

        public GameState Start()
        {
            if (!HasEnoughPlayers())
            {
                return null;
            }

            Over();

            GameState gameState = new GameState();
            gameState.Phase = GamePhase.Start;
            var cards = new List<GameCard>(Room.Cards);
            for (var i = -CENTER_CARD_NUM; cards.Count > 0; i++)
            {
                int cardIdx = random.Next(cards.Count);
                gameState.Seats.Add(i, new GameSeat() { No = i, Card = cards[cardIdx], Dead = false, DeadReason = DeadReason.None, Win = false, Option = null, Vote = 0 });
                cards.RemoveAt(cardIdx);
            }
            Room.StateStack.Add(gameState);
            return gameState;
        }

        #endregion

        public List<GameOption> GetOptions(int seatNo)
        {
            var gamePhase = GetCurrentPhase();
            if (gamePhase == GamePhase.Start) 
            {
                return GetOptionsAtStart(seatNo);
            }
            else if(gamePhase == GamePhase.Over)
            {
                return GetOptionsAtOver(seatNo);
            }
            else if (gamePhase == GamePhase.Day)
            {
                return GetOptionsAtDay(seatNo);
            }
            else
            {
                return GetOptionsAtNight(seatNo);
            }
        }

        protected List<GameOption> GetOptionsAtStart(int seatNo)
        {
            var initSeat = Room.StateStack.First().Seats[seatNo];
            var currSeat = Room.StateStack.Last().Seats[seatNo];
            if(currSeat.Option != null) 
            {
                return new List<GameOption>();
            }

            List<GameOption> options = new List<GameOption>();
            options.Add(new GameOption() { Phase = GamePhase.Start, Command = GameCommand.Identify, Arguments = new object[] { initSeat.No } });
            return options;
        }

        protected List<GameOption> GetOptionsAtOver(int seatNo)
        {
            if(Room.StateStack == null || Room.StateStack.Count == 0)
            {
                return new List<GameOption>();
            }
            var initSeat = Room.StateStack.First().Seats[seatNo];
            var currSeat = Room.StateStack.Last().Seats[seatNo];
            if (currSeat.Option != null)
            {
                return new List<GameOption>();
            }

            List<GameOption> options = new List<GameOption>();
            options.Add(new GameOption() { Phase = GamePhase.Over, Command = GameCommand.Result, Arguments = new object[] { initSeat.No } });
            return options;
        }

        protected List<GameOption> GetOptionsAtDay(int seatNo)
        {
            var initSeat = Room.StateStack.First().Seats[seatNo];
            var currSeat = Room.StateStack.Last().Seats[seatNo];
            var playerCount = Room.Cards.Count - CENTER_CARD_NUM;
            if (currSeat.Option != null)
            {
                return new List<GameOption>();
            }

            var gamePhase = GetCurrentPhase();

            List<GameOption> options = new List<GameOption>();
            for (var no = 0; no < playerCount; no++)
            {
                if (no != initSeat.No)
                {
                    options.Add(new GameOption() { Phase = GamePhase.Day, Command = GameCommand.Vote, Arguments = new object[] { no } });
                }
            }
            return options;
        }

        protected GameRole GetPlayRole(GameCard card)
        {
            if (card.Role != GameRole.Doppelgänger)
            {
                return card.Role;
            }
            if(!string.IsNullOrEmpty(card.Extra) && Enum.TryParse(card.Extra, out GameRole role))
            {
                return role;
            }
            return GameRole.Doppelgänger;
        }

        protected List<GameOption> GetOptionsAtNight(int seatNo)
        {
            var initSeat = Room.StateStack.First().Seats[seatNo];
            var currSeat = Room.StateStack.Last().Seats[seatNo];
            var playerCount = Room.Cards.Count - CENTER_CARD_NUM;
            if (currSeat.Option != null)
            {
                return new List<GameOption>();
            }

            var gamePhase = GetCurrentPhase();

            List<GameOption> options = new List<GameOption>();
            switch (gamePhase)
            {
                case GamePhase.NightDoppelgängerFeign:
                    if (initSeat.Card.Role == GameRole.Doppelgänger)
                    {
                        for (var no = 0; no < playerCount; no++)
                        {
                            if(no != initSeat.No)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Feign, Arguments = new object[] { initSeat.No, no } });
                            }
                        }
                    }
                    break;
                case GamePhase.NightDoppelgängerPlay:
                    if(initSeat.Card.Role == GameRole.Doppelgänger) 
                    {
                        if(GetPlayRole(initSeat.Card) == GameRole.Seer)
                        {
                            for (var i = 1; i <= CENTER_CARD_NUM; i++)
                            {
                                for(var j = i+1;j<=CENTER_CARD_NUM;j++)
                                {
                                    options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.See, Arguments = new object[] { -i, -j } });
                                }
                            }
                            for (var no = 0; no < playerCount; no++)
                            {
                                if (no != initSeat.No)
                                {
                                    options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.See, Arguments = new object[] { no } });
                                }
                            }
                        }
                        else if(GetPlayRole(initSeat.Card) == GameRole.Robber)
                        {
                            for (var no = 0; no < playerCount; no++)
                            {
                                if (no != initSeat.No)
                                {
                                    options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Rob, Arguments = new object[] { initSeat.No, no } });
                                }
                            }
                        }
                        else if (GetPlayRole(initSeat.Card) == GameRole.Troublemaker)
                        {
                            for (var no1 = 0; no1 < playerCount; no1++)
                            {
                                if (initSeat.No == no1) continue;
                                for (var no2 = no1 + 1; no2 < playerCount; no2++)
                                {
                                    if (initSeat.No == no2) continue;
                                    options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Exchange, Arguments = new object[] { no1, no2 } });
                                }
                            }
                        }
                        else if (GetPlayRole(initSeat.Card) == GameRole.Drunk)
                        {
                            for(var i= 1; i <= CENTER_CARD_NUM; i++)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Drunk, Arguments = new object[] { initSeat.No, -i } });
                            }
                        }
                        else if (GetPlayRole(initSeat.Card)  == GameRole.Minion)
                        {
                            options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Locate, Arguments = new object[] { GameRole.Werewolf } });
                        }
                    }
                    break;

                case GamePhase.NightWerewolf:
                    if (initSeat.Card.Role == GameRole.Werewolf || initSeat.Card.Extra == GameRole.Werewolf.ToString()) {
                        var aloneWerewolf = Room.StateStack.First().Seats.Values
                            .Count(seat => seat.No >= 0 && GetPlayRole(seat.Card) == GameRole.Werewolf) == 1;
                        if (aloneWerewolf)
                        {
                            for(var i = 1; i <= CENTER_CARD_NUM; i++)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.See, Arguments = new object[] { -i } });
                            }
                        }
                        else
                        {
                            options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Locate, Arguments = new object[] { GameRole.Werewolf } });
                        }
                    }
                    break;
                case GamePhase.NightMinion:
                    if (initSeat.Card.Role == GameRole.Minion)
                    {
                        options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Locate, Arguments = new object[] { GameRole.Werewolf } });
                    }
                    break;
                case GamePhase.NightMasons:
                    if(initSeat.Card.Role == GameRole.Masons || initSeat.Card.Extra == GameRole.Masons.ToString())
                    {
                        options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Locate, Arguments = new object[] { GameRole.Masons } });
                    }
                    break;
                case GamePhase.NightSeer:
                    if (initSeat.Card.Role == GameRole.Seer)
                    {

                        for (var i = 1; i <= CENTER_CARD_NUM; i++)
                        {
                            for (var j = i + 1; j <= CENTER_CARD_NUM; j++)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.See, Arguments = new object[] { -i, -j } });
                            }
                        }
                        for (var no = 0; no < playerCount; no++)
                        {
                            if (no != initSeat.No)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.See, Arguments = new object[] { no } });
                            }
                        }
                    }
                    break;
                case GamePhase.NightRobber:
                    if (initSeat.Card.Role == GameRole.Robber)
                    {
                        for (var no = 0; no < playerCount; no++)
                        {
                            if (no != initSeat.No)
                            {
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Rob, Arguments = new object[] { initSeat.No, no } });
                            }
                        }
                    }
                    break;
                case GamePhase.NightTroublemaker:
                    if (initSeat.Card.Role == GameRole.Troublemaker)
                    {
                        for(var no1 = 0; no1 < playerCount; no1++)
                        {
                            if (initSeat.No == no1) continue;
                            for (var no2 = no1 + 1; no2 < playerCount; no2++)
                            {
                                if (initSeat.No == no2) continue;
                                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Exchange, Arguments = new object[] { no1, no2 } });
                            }
                        }
                    }
                    break;
                case GamePhase.NightDrunk:
                    if (initSeat.Card.Role == GameRole.Drunk)
                    {
                        for (var i = 1; i <= CENTER_CARD_NUM; i++)
                        {
                            options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Drunk, Arguments = new object[] { initSeat.No, -i } });
                        }
                    }
                    break;
                case GamePhase.NightInsomniac:
                    if (initSeat.Card.Role == GameRole.Insomniac)
                    {
                        options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Identify, Arguments = new object[] { initSeat.No } });
                    }
                    break;
                case GamePhase.NightDoppelgängerInsomniac:
                    if (initSeat.Card.Role == GameRole.Doppelgänger 
                        && GetPlayRole(initSeat.Card) == GameRole.Doppelgänger)
                    {
                        options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.Identify, Arguments = new object[] { initSeat.No } });
                    }
                    break;
            }
            if (options.Count == 0)
            {
                options.Add(new GameOption() { Phase = gamePhase, Command = GameCommand.None });
            }
            return options;
        }


        public GamePhase GetCurrentPhase()
        {
            if (Room.StateStack.Count == 0) return GamePhase.Over;
            return Room.StateStack.Last().Phase;
        }

        protected GamePhase GetNextPhase()
        {
            var phases = GetAllPhases();
            var current = GetCurrentPhase();
            if (current == GamePhase.Over) return GamePhase.Over;
            var currentIndex = phases.FindIndex(phrase => phrase == current);
            return phases[currentIndex + 1];
        }

        protected bool CanNextPhase()
        {
            return Room.StateStack.Last().Seats.Values.All(seat => seat.No < 0 || seat.Option != null);
        }

        public GameOption Action(int seatNo, GameOption option)
        {
            var currentSeat = Room.StateStack.Last().Seats[seatNo];
            if (currentSeat.Option != null) return currentSeat.Option;


            List<int> seatNos = null;
            GameSeat seat0 = null;
            GameSeat seat1 = null;
            GameCard cardTemp = null;
            switch (option.Command)
            {
                case GameCommand.Identify:
                case GameCommand.See:
                    seatNos = option.Arguments.Select(a => Convert.ToInt32(a)).ToList();
                    var roles = new object[seatNos.Count];
                    if (seatNos.Count > 0)
                    {
                        for(var i = 0; i < seatNos.Count; i++)
                        {
                            roles[i] = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[i] == seat.No).Card.Role;
                        }
                    }
                    option.Result = roles;
                    break;
                case GameCommand.Feign:
                    seatNos = option.Arguments.Select(a => Convert.ToInt32(a)).ToList();
                    seat0 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[0] == seat.No);
                    seat1 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[1] == seat.No);
                    seat0.Card.Extra = seat1.Card.Role.ToString();
                    option.Result = new object[] { seat1.Card.Role };
                    break;
                case GameCommand.Locate:
                    if((GameRole)Convert.ToInt32(option.Arguments[0]) == GameRole.Werewolf)
                    {
                        var seats = Room.StateStack.First().Seats.Values
                            .Where(seat => seat.No != seatNo && seat.No >= 0 
                                        && GetPlayRole(seat.Card) == GameRole.Werewolf)
                            .ToList();
                        seatNos = seats.Select(seat => seat.No).ToList();
                        option.Result = seatNos.Cast<object>().ToArray();
                    }
                    else if((GameRole)Convert.ToInt32(option.Arguments[0]) == GameRole.Masons)
                    {
                        var seats = Room.StateStack.First().Seats.Values
                            .Where(seat => seat.No != seatNo && seat.No >= 0 
                                        && GetPlayRole(seat.Card) == GameRole.Masons )
                            .ToList();
                        seatNos = seats.Select(seat => seat.No).ToList();
                        option.Result = seatNos.Cast<object>().ToArray();
                    }
                    break;
                case GameCommand.Rob:
                    seatNos = option.Arguments.Select(a => Convert.ToInt32(a)).ToList();
                    seat0 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[0] == seat.No);
                    seat1 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[1] == seat.No);
                    cardTemp = seat0.Card;
                    seat0.Card = seat1.Card;
                    seat1.Card = cardTemp;
                    option.Result = new object[] { seat0.Card.Role };
                    break;
                case GameCommand.Drunk:
                case GameCommand.Exchange:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    seat0 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[0] == seat.No);
                    seat1 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[1] == seat.No);
                    cardTemp = seat0.Card;
                    seat0.Card = seat1.Card;
                    seat1.Card = cardTemp;
                    option.Result = new object[] { "done" };
                    break;
                case GameCommand.Vote:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    Room.StateStack.Last().Seats.Values.ToList().ForEach(seat =>
                    {
                        if (seatNos.Contains(seat.No))
                        {
                            seat.Vote++;
                        }
                    });
                    option.Result = new object[] { "done" };
                    break;
                case GameCommand.Result:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    seat0 = Room.StateStack.Last().Seats.Values.FirstOrDefault(seat => seatNos[0] == seat.No);
                    option.Result = new object[] { seat0.Win, seat0.Card.Role, GetPlayRole(seat0.Card), seat0.Dead, seat0.DeadReason};
                    break;
                case GameCommand.None:
                    option.Result = new object[] { "done" };
                    break;
            }

            var player = GetPlayerBySeatNo(seatNo);
            player.HistoryOptions.Add(option);
            currentSeat.Option = option;


            if (CanNextPhase()) {
                if (GetCurrentPhase() == GamePhase.Day)
                {
                    var maxVote = Room.StateStack.Last().Seats.Values.Max(seat => seat.Vote);
                    if(maxVote > 1)
                    {
                        var deadSeats = Room.StateStack.Last().Seats.Values.Where(seat => seat.Vote == maxVote).ToList();
                        deadSeats.ForEach(seat => {
                            seat.Dead = true;
                            seat.DeadReason = DeadReason.Vote;
                            if (GetPlayRole(seat.Card) == GameRole.Hunter)
                            {
                                Room.StateStack.Last().Seats.Values.ToList().ForEach(voteSeat => {
                                    if (voteSeat.No >= 0 && Convert.ToInt32(voteSeat.Option.Arguments[0]) == seat.No)
                                    {
                                        voteSeat.Dead = true;
                                        voteSeat.DeadReason = DeadReason.Hunter;
                                    }
                                });
                            }
                        });
                    }
                    var isVillageWin = JudgeVillageWin();
                    var isWerewolfWin = JudgeWerewolfWin();
                    Room.StateStack.Last().Seats.Values.ToList().ForEach(seat => { 
                        switch(seat.Card.Role)
                        {
                            case GameRole.Tanner:
                                seat.Win = seat.Dead;
                                break;
                            case GameRole.Doppelgänger:
                                if(seat.Card.Extra == GameRole.Tanner.ToString())
                                {
                                    seat.Win = seat.Dead;
                                } 
                                else if(seat.Card.Extra == GameRole.Werewolf.ToString() || seat.Card.Extra == GameRole.Minion.ToString())
                                {
                                    seat.Win = isWerewolfWin;
                                }
                                else
                                {
                                    seat.Win = isVillageWin;
                                }
                                break;
                            case GameRole.Werewolf:
                            case GameRole.Minion:
                                seat.Win = isWerewolfWin;
                                break;
                            default:
                                seat.Win = isVillageWin;
                                break;
                        }
                    });
                }
                if (GetCurrentPhase() != GamePhase.Over)
                {
                    var clone = CloneGameState(Room.StateStack.Last());
                    clone.Phase = GetNextPhase();
                    Room.StateStack.Add(clone);
                }
            }
            return option;
        }

        protected GameState CloneGameState(GameState state)
        {
            var clone = new GameState();
            clone.Seats = new Dictionary<int, GameSeat>();
            foreach (var seat in state.Seats.Values)
            {
                clone.Seats.Add(seat.No, new GameSeat() { No = seat.No, Card = seat.Card, Vote = seat.Vote, Dead = seat.Dead, Win = seat.Win });
            }
            return clone;
        }

        protected bool IsNoneWerewolf()
        {
            var noneWerewolf = Room.StateStack.Last().Seats.Values
                .Where(seat => seat.No >= 0)
                .All(seat => GetPlayRole(seat.Card) != GameRole.Werewolf);

            return noneWerewolf;
        }

        protected bool IsNoneMinion()
        {
            var noneMinion = Room.StateStack.Last().Seats.Values
                .Where(seat => seat.No >= 0)
                .All(seat => GetPlayRole(seat.Card) != GameRole.Minion);

            return noneMinion;
        }

        protected bool JudgeVillageWin()
        {
            var noneWerewolf = IsNoneWerewolf();
            var noneMinion = IsNoneMinion();
            if(noneWerewolf && noneMinion)
            {
                // 没狼人，没爪牙：不能死人
                var villageWin = Room.StateStack.Last().Seats.Values
                    .All(seat => !seat.Dead);
                return villageWin;
            }
            else if (noneWerewolf && !noneMinion)
            {
                // 没狼人，有爪牙：死一个爪牙
                var villageWin = Room.StateStack.Last().Seats.Values
                    .Where(seat => seat.No >= 0 
                        && GetPlayRole(seat.Card) == GameRole.Minion)
                    .Any(seat => seat.Dead);
                return villageWin;
            }
            else
            {
                // 有狼人：死一个狼人
                var villageWin = Room.StateStack.Last().Seats.Values
                    .Where(seat => seat.No >= 0
                        && GetPlayRole(seat.Card) == GameRole.Werewolf)
                    .Any(seat => seat.Dead);
                return villageWin;
            }
        }

        protected bool JudgeWerewolfWin()
        {
            var noneWerewolf = IsNoneWerewolf();
            var noneMinion = IsNoneMinion();

            var werewolfWin = false;
            if (noneWerewolf && noneMinion)
            {
                // 没狼人，没爪牙：输
                return werewolfWin;
            }
            else if (noneWerewolf && !noneMinion)
            {
                // 没狼人，有爪牙：没爪牙死 且 不能只死了皮匠
                var noneMinionDead = Room.StateStack.Last().Seats.Values
                .Where(seat => seat.No >= 0
                    && GetPlayRole(seat.Card) == GameRole.Minion)
                .All(seat => !seat.Dead);
                if (!noneMinionDead) werewolfWin = false;

                var deadCount = Room.StateStack.Last().Seats.Values
                        .Where(seat => seat.Dead).Count();
                if (deadCount == 0) werewolfWin = true;
                else
                {
                    var onlyTannerDead = Room.StateStack.Last().Seats.Values
                            .Where(seat => seat.Dead)
                            .All(seat => GetPlayRole(seat.Card) == GameRole.Tanner);
                    if (onlyTannerDead) werewolfWin = false;
                    else werewolfWin = true;
                }
                return werewolfWin;
            }
            else
            {
                // 有狼人：没狼人死 且 不能只死了皮匠
                var noneWerewolfDead = Room.StateStack.Last().Seats.Values
                    .Where(seat => seat.No >= 0 && GetPlayRole(seat.Card) == GameRole.Werewolf)
                    .All(seat => !seat.Dead);
                if (!noneWerewolfDead) werewolfWin = false;
                var deadCount = Room.StateStack.Last().Seats.Values
                        .Where(seat => seat.Dead).Count();
                if (deadCount == 0) werewolfWin = true;
                else
                {
                    var onlyTannerDead = Room.StateStack.Last().Seats.Values
                            .Where(seat => seat.Dead)
                            .All(seat => GetPlayRole(seat.Card) == GameRole.Tanner);
                    if (onlyTannerDead) werewolfWin = false;
                    else werewolfWin = true;
                }
                return werewolfWin;
            }
        }

        public Player GetPlayerByUserId(string userId)
        {
            return Room.Players.FirstOrDefault(player => player.UserId == userId);
        }

        public Player GetPlayerBySeatNo(int seatNo)
        {
            return Room.Players.FirstOrDefault(player => player.SeatNo == seatNo);
        }

        public List<Player> GetPlayers()
        {
            return Room.Players.ToList();
        }

        public bool IsRoomMaster(string userId)
        {
            return Room.Players?.FirstOrDefault()?.UserId == userId;
        }

        public string GetRoleDesc(GameRole role)
        {
            switch (role)
            {
                case GameRole.Village:
                    return "村民";
                case GameRole.Werewolf:
                    return "狼人";
                case GameRole.Minion:
                    return "爪牙";
                case GameRole.Masons:
                    return "守夜人";
                case GameRole.Seer:
                    return "预言家";
                case GameRole.Robber:
                    return "强盗";
                case GameRole.Troublemaker:
                    return "捣蛋鬼";
                case GameRole.Drunk:
                    return "酒鬼";
                case GameRole.Insomniac:
                    return "失眠者";
                case GameRole.Hunter:
                    return "猎人";
                case GameRole.Tanner:
                    return "皮匠";
                case GameRole.Doppelgänger:
                    return "幽灵";

            }
            return role.ToString();
        }

        public string GetPhaseDesc(GamePhase phase)
        {
            switch(phase)
            {
                case GamePhase.Start:
                    return "游戏开始";
                case GamePhase.Day:
                    return "天亮了，请找出狼人";
                case GamePhase.Over:
                    return "游戏结束";
                case GamePhase.NightDoppelgängerFeign:
                    return $"{GetRoleDesc(GameRole.Doppelgänger)}化身";
                case GamePhase.NightDoppelgängerPlay:
                    return $"{GetRoleDesc(GameRole.Doppelgänger)}行动";
                case GamePhase.NightWerewolf:
                    return $"{GetRoleDesc(GameRole.Werewolf)}行动";
                case GamePhase.NightMinion:
                    return $"{GetRoleDesc(GameRole.Minion)}行动";
                case GamePhase.NightMasons:
                    return $"{GetRoleDesc(GameRole.Masons)}行动";
                case GamePhase.NightSeer:
                    return $"{GetRoleDesc(GameRole.Seer)}行动";
                case GamePhase.NightRobber:
                    return $"{GetRoleDesc(GameRole.Robber)}行动";
                case GamePhase.NightTroublemaker:
                    return $"{GetRoleDesc(GameRole.Troublemaker)}行动";
                case GamePhase.NightDrunk:
                    return $"{GetRoleDesc(GameRole.Drunk)}行动";
                case GamePhase.NightInsomniac:
                    return $"{GetRoleDesc(GameRole.Insomniac)}行动";
                case GamePhase.NightDoppelgängerInsomniac:
                    return $"{GetRoleDesc(GameRole.Doppelgänger)}{GetRoleDesc(GameRole.Insomniac)}行动";
            }
            return phase.ToString();
        }

        public string GetOptionResultDesc(GameOption option)
        {
            switch (option.Command)
            {
                case GameCommand.Identify:
                case GameCommand.See:
                    var roles = option.Result.Select(a=>Convert.ToInt32(a)).Cast<GameRole>();
                    var roleInfo = string.Join(" 和 ", roles.Select(role => GetRoleDesc(role)));
                    return $"{roleInfo}";
                case GameCommand.Feign:
                    return GetRoleDesc((GameRole)Convert.ToInt32(option.Result[0]));

                case GameCommand.Locate:
                    var seatNos = option.Result.Select(a=>Convert.ToInt32(a));
                    var playerInfo = string.Join(" 和 ", seatNos.Select(seatNo => {
                        var player = GetPlayerBySeatNo(seatNo);
                        return $"[P{player.SeatNo}]{player.UserNick}";
                    }));
                    return $"{playerInfo}";
                case GameCommand.Rob:
                    return $"新身份是[{GetRoleDesc((GameRole)Convert.ToInt32(option.Result[0]))}]";
                case GameCommand.Exchange:
                case GameCommand.Drunk:
                case GameCommand.Vote:
                case GameCommand.None:
                    return $"完成";
                case GameCommand.Result:
                    var roleName = GetRoleDesc((GameRole)Convert.ToInt32(option.Result[1]));
                    if(option.Result[1] != option.Result[2])
                    {
                        roleName = GetRoleDesc((GameRole)Convert.ToInt32(option.Result[2]));
                    }
                    var status = "";
                    if ((bool)option.Result[3])
                    {
                        var reason =(DeadReason)Convert.ToInt32(option.Result[4]);
                        status = "死于";
                        if(reason == DeadReason.Hunter)
                        {
                            status += "猎人";
                        }
                        if(reason == DeadReason.Vote)
                        {
                            status += "投票";
                        }
                    }
                    else
                    {
                        status = "幸存";

                    }
                    return $"{((bool)option.Result[0]?"胜利":"失败")}[{roleName}{status}]";
            }
            return "";
        }

        public string GetOptionDesc(GameOption option)
        {
            List<int> seatNos = null;
            string playerInfo = null;
            switch (option.Command)
            {
                case GameCommand.Identify:
                    return $"我的身份";
                case GameCommand.See:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    playerInfo = string.Join(" 和 ", seatNos.Select(seatNo => {
                        if (seatNo < 0)
                        {
                            return $"[中{-seatNo}]";
                        }
                        else
                        {
                            var player = GetPlayerBySeatNo(seatNo);
                            return $"[P{player.SeatNo}]{player.UserNick}";
                        }
                    }));
                    return $"查看 {playerInfo}";
                case GameCommand.Feign:
                    seatNos = option.Arguments.Select(a => Convert.ToInt32(a)).ToList();
                    return $"化身为 [P{seatNos[1]}]{GetPlayerBySeatNo(seatNos[1]).UserNick}";

                case GameCommand.Locate:
                    if ((GameRole)Convert.ToInt32(option.Arguments[0]) == GameRole.Werewolf)
                    {
                        return $"寻找 {GetRoleDesc(GameRole.Werewolf)} 同伴";
                    }
                    else if((GameRole)Convert.ToInt32(option.Arguments[0]) == GameRole.Masons)
                    {
                        return $"寻找 {GetRoleDesc(GameRole.Masons)} 同伴";
                    }
                    break;
                case GameCommand.Rob:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    return $"与 [P{seatNos[1]}]{GetPlayerBySeatNo(seatNos[1]).UserNick} 交换";
                case GameCommand.Drunk:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();

                    return $"与 [中{-seatNos[1]}]交换";
                case GameCommand.Exchange:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();

                    playerInfo = string.Join(" 和 ", seatNos.Select(seatNo => {
                        var player = GetPlayerBySeatNo(seatNo);
                        return $"[P{player.SeatNo}]{player.UserNick}";
                    }));
                    return $"交换 {playerInfo} ";
                case GameCommand.Vote:
                    seatNos = option.Arguments.Select(a=>Convert.ToInt32(a)).ToList();
                    playerInfo = string.Join(" 和 ", seatNos.Select(seatNo => {
                        var player = GetPlayerBySeatNo(seatNo);
                        return $"[P{player.SeatNo}]{player.UserNick}";
                    }));
                    return $"投 {playerInfo}";
                case GameCommand.Result:
                    return "游戏结果";
                case GameCommand.None:
                    return "无行动";
            }
            return "";
        }

        public string GetMyGameInfo(int seatNo)
        {
            var player = GetPlayerBySeatNo(seatNo);
            string info = $"房间号：{Room.Id}\n";
            info += "玩家列表：";
            foreach(var p in Room.Players)
            {
                info += $"[P{p.SeatNo}]{p.UserNick + (p.UserId == player.UserId ? "(我)" : "" )} ";
            }
            info += "\n";
            info += "= = = = = = = =\n";

            foreach (var op in player.HistoryOptions)
            {
                if (op.Command == GameCommand.None) continue;
                info +=  $"{GetPhaseDesc(op.Phase)}：{GetOptionDesc(op)}{(op.Result == null ? "" : " => " + GetOptionResultDesc(op))}\n";
            }
            return info;
        }
    }
}
