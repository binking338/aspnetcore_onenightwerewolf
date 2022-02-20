using System;
namespace OneNightWerewolf.Core
{
    public class Grave : ICardPlace
    {
        public Grave(string no)
        {
            No = no;
        }

        public string No { get; private set; }

        public ICard OriginCard { get; set; }

        public ICard FinalCard { get; set; }
    }
}
