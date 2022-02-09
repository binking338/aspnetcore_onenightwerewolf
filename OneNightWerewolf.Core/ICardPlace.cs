using System;
namespace OneNightWerewolf.Core
{
    public interface ICardPlace
    {
        public ICard OriginCard { get; set; }

        public ICard FinalCard { get; set; }

        public void SeeCard(IMonitor monitor);

        public virtual void SwapCardWith(ICardPlace cardPlace)
        {
            var card = FinalCard;
            FinalCard = cardPlace.FinalCard;
            cardPlace.FinalCard = card;
        }

        public virtual void PutCard(ICard card)
        {
            OriginCard = card;
            FinalCard = card;
        }

        public virtual void RecycleCard()
        {
            OriginCard = null;
            FinalCard = null;
        }
    }
}
