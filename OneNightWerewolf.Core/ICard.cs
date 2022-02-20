using System;
using System.Linq;
namespace OneNightWerewolf.Core
{
    public interface ICard
    {
        string Id { get; set; }
        string No { get; }

        string Name { get; }

        Role Role { get; }

        IAbility[] Abilities { get; }

        IRound[] Rounds { get; }

        public virtual void Reset()
        {
            ;
        }

        public virtual bool JudgeWinning(Table table, string seatNo)
        {
            var seat = table.GetSeats().FirstOrDefault(s => s.No == seatNo);
            if(seat == null)
            {
                throw new ArgumentOutOfRangeException(nameof(seatNo));
            }
            switch (Role)
            {
                case Role.Tanner:
                    return seat.Dead;
                case Role.Werewolf:
                case Role.Minion:
                    return table.WinningCamp == Camp.Werewolf;
                default:
                    return table.WinningCamp == Camp.Villiage;
            }
        }
    }
}
