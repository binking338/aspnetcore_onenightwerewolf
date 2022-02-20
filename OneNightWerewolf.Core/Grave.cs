using System;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public class Grave : ICardPlace
    {
        public Table _table;

        public Grave(string no)
        {
            No = no;
        }
        public string OriginCardId { get; set; }

        public string FinalCardId { get; set; }

        public string No { get; set; }

        public void Clone(Grave grave)
        {
            this.OriginCardId = grave.OriginCardId;
            this.FinalCardId = grave.FinalCardId;
            this.No = grave.No;
        }

        public Grave Assosiate(Table table)
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
        }
    }
}
