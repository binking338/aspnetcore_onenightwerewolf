using System;
namespace OneNightWerewolf.Core
{
    public interface ICardPlace
    {
        public string OriginCardId { get; set; }
        public string FinalCardId { get; set; }

        public ICard GetOriginCard();

        public ICard GetFinalCard();

        public void SwapCardWith(ICardPlace cardPlace);

        public void PutCard(ICard card);

        public void RecycleCard();
    }
}
