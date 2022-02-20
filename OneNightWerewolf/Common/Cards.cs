using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Common
{
    public static class Cards
    {
        public static ICard[] CreateCards(string cardNo)
        {
            List<ICard> cards = new List<ICard>();
            var i = 0;
            foreach(var no in cardNo.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var card = CreateCard(no);
                if (card == null) continue;
                card.Id = $"{i:##}-{no}";
                cards.Add(card);
            }
            return cards.ToArray();
        }

        public static ICard[] AllCards()
        {
            return CreateCards("01,02,03,04,05,06,07,08,09,10,11,12");
        }

        private static ICard CreateCard(string cardNo)
        {
            switch (cardNo)
            {
                case "01":
                    return new Village.VillageCard();
                case "02":
                    return new Werewolf.WerewolfCard();
                case "03":
                    return new Minion.MinionCard();
                case "04":
                    return new Mason.MasonCard();
                case "05":
                    return new Seer.SeerCard();
                case "06":
                    return new Robber.RobberCard();
                case "07":
                    return new Troublemaker.TroublemakerCard();
                case "08":
                    return new Drunk.DrunkCard();
                case "09":
                    return new Insomniac.InsomniacCard();
                case "10":
                    return new Hunter.HunterCard();
                case "11":
                    return new Tanner.TannerCard();
                case "12":
                    return new Doppelganger.DoppelgangerCard();
            }
            return null;
        }
    }
}
