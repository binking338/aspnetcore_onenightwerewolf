using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Room
    {
        private Table _table;
        private Game _game;

        public Room(string name, IRound[] basicRounds, IActionHandler[] actionHandlers, IWinningCampDecisionRule winningCampDecisionRule)
        {
            CreateTime = DateTime.Now;
            Name = name;

            _game = new Game(basicRounds, actionHandlers, winningCampDecisionRule);
            _table = new Table(_game);
            Players = new List<Player>();
        }

        public string Name { get; private set; }

        public string GameConfig { get; private set; }

        public DateTime CreateTime { get; private set; }

        public List<Player> Players { get; private set; }

        public int GetPlayerLimit()
        {
            return (_game?.Cards?.Length ?? 3) - 3;
        }

        public Player GetHolder()
        {
            return Players?.FirstOrDefault();
        }

        public Table GetTable()
        {
            return this._table;
        }

        public Game GetGame()
        {
            return this._game;
        }

        public bool Config(ICard[] cards)
        {
            if (!GetTable().IsGameFinished())
            {
                return false;
            }
            GetGame().New(cards);
            GetTable().Config();
            GameConfig = string.Join(",", cards.Select(c => c.No));
            for (int i = 0; i < Players.Count && i < GetPlayerLimit(); i++)
            {
                GetTable().Takeseat(Players[i].Nick);
            }
            return true;
        }

        public bool TakeIn(Player player)
        {
            if (!Players.Any(u => u.Id == player.Id))
            {
                Players.Add(player);
                if (!GetTable().IsAllSeatTaken())
                {
                    GetTable().Takeseat(player.Nick);
                }
            }

            return true;
        }

        public bool TakeOut(Player player)
        {
            if (Players.Any(u => u.Id == player.Id))
            {
                player = Players.First(u => u.Id == player.Id);
                Players.Remove(player);
                GetTable().Disseat(player.Nick);
                return true;
            }
            return false;
        }

        public bool HasPlayer(string playerId)
        {
            return Players.Any(p => p.Id == playerId);
        }

        public Player FindPlayer(string playerId)
        {
            return Players.FirstOrDefault(p => p.Id == playerId);
        }

        public Player FindPlayerByNick(string nick)
        {
            return Players.FirstOrDefault(p => p.Nick == nick);
        }

        public bool Start()
        {
            if (!GetTable().IsAllSeatReady())
            {
                return false;
            }
            if (!GetTable().IsGameFinished())
            {
                return false;
            }
            GetTable().NewGame();
            GetTable().Recycle();
            GetGame().Deal(GetTable());
            GetTable().NextRound();
            return true;
        }

        public void ForceStop()
        {
            GetTable().NewGame();
            GetTable().Recycle();
        }

        private void PollingCheck()
        {
            if (GetTable().IsRoundFinished())
            {
                GetTable().NextRound();
            }
        }

        public void Action(Player player, Choice choice)
        {
            if (!GetTable().MakeChoice(GetTable().FindSeatByNick(player.Nick).No, choice))
            {
                throw new InvalidOperationException("不能重复选择操作选项");
            }
            PollingCheck();
        }

        public IDictionary<string, Choice> Choices(Player player)
        {
            var choices = GetTable().GetChoices(GetTable().FindSeatByNick(player.Nick).No);
            return choices;
        }

    }
}
