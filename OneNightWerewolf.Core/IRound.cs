using System;
namespace OneNightWerewolf.Core
{
    public interface IRound
    {
        public Phase Phase { get; }

        public int Order { get; }

        public string Name { get; }

        public virtual bool Enabled(Table table)
        {
            return true;
        }
    }
}
