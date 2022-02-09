using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Room
    {
        public Room(string name, IRound[] basicRounds, IActionHandler[] actionHandlers, IWinningCampDecisionRule winningCampDecisionRule)
        {
            Name = name;
            Game = new Game(basicRounds, actionHandlers, winningCampDecisionRule);
            Table = new Table(Game);
            Players = new List<Player>();
            CreateTime = DateTime.Now;
        }

        public string Name { get; }

        public Table Table { get; }

        public Game Game { get; }

        public DateTime CreateTime { get; private set; }

        public IList<Player> Players { get; }

        public int PlayerLimit { get; private set; }

        public Player Holder { get; private set; }

        public bool Config(ICard[] cards)
        {
            if (!Table.IsGameFinished())
            {
                return false;
            }
            PlayerLimit = cards.Length - 3;
            Game.New(cards);
            Table.Config(PlayerLimit, 3);
            for (int i = 0; i < Players.Count && i < PlayerLimit; i++)
            {
                Table.Takeseat(Players[i].Nick);
            }
            return true;
        }

        public bool TakeIn(Player player)
        {
            if(!Players.Any(u => u.Id == player.Id))
            {
                Players.Add(player);
                if(Players.Count == 1)
                {
                    Holder = player;
                }
                if (!Table.IsAllSeatTaken())
                {
                    Table.Takeseat(player.Nick);
                }
            }

            return true;
        }

        public bool TakeOut(Player player)
        {
            if (Players.Any(u => u.Id == player.Id))
            {
                player = Players.First(u => u.Id == player.Id);
                if(player == Holder)
                {
                    Holder = null;
                }
                Players.Remove(player);
                if(Holder == null)
                {
                    Holder = Players.FirstOrDefault();
                }
                Table.Disseat(player.Nick);
                return true;
            }
            return false;
        }

        public bool Start()
        {
            if (!Table.IsAllSeatTaken())
            {
                return false;
            }
            if (!Table.IsGameFinished())
            {
                return false;
            }
            Table.NewGame();
            Table.Recycle();
            Game.Deal(Table);
            Table.NextRound();
            return true;
        }

        public void Action(Player player, Choice choice)
        {
            if(!Table.MakeChoice(player.Nick, choice))
            {
                throw new InvalidOperationException("不能重复选择操作选项");
            }
            PollingCheck();
        }

        public IDictionary<string, Choice> Choices(Player player)
        {
            var choices = Table.GetChoices(player.Nick);
            return choices;
        }

        public void ForceStop()
        {
            Table.NewGame();
            Table.Recycle();
        }

        private void PollingCheck()
        {
            if (Table.IsRoundFinished())
            {
                Table.NextRound();
            }
        }

    }
}
