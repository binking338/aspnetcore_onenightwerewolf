using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Game
    {
        private readonly Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

        public Game(IRound[] basicRounds, IActionHandler[] actionHandlers)
        {
            BasicRounds = basicRounds;
            ActionHandlers = actionHandlers;
        }

        public IActionHandler[] ActionHandlers { get; private set; }

        public IRound[] BasicRounds { get; private set; }

        public ICard[] Cards { get; private set; }

        public IRound[] Rounds { get; private set; }

        public void New(ICard[] cards)
        {
            Cards = cards;
            List<IRound> rounds = BasicRounds.ToList();
            foreach (var card in cards)
            {
                rounds.AddRange(card.Rounds);
            }
            var map = new Dictionary<string, IRound>();
            foreach(var round in rounds)
            {
                map[round.Name] = round;
            }
            rounds = map.Values.ToList();
            rounds.Sort((a, b) => (a.Phase - b.Phase) * 10000 + a.Order - b.Order);
            Rounds = rounds.ToArray();
        }

        public void Deal(Table table)
        {
            table.Deal(Shuffle());
        }

        private ICard[] Shuffle()
        {
            var cards = new List<ICard>(Cards);
            var shuffledCards = new ICard[Cards.Length];
            for (int i = 0; i < Cards.Length; i++)
            {
                var r = random.Next(cards.Count);
                shuffledCards[i] = cards[r];
                shuffledCards[i].Reset();
                cards.RemoveAt(r);
            }
            return shuffledCards;
        }
    }
}
