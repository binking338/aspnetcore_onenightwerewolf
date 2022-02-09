using System;
using System.Linq;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Common
{
    public class DefaultWinningCampDecisionRule : IWinningCampDecisionRule
    {
        public Camp Judge(Table table)
        {
            var deaths = table.Seats.Where(s => s.Dead);
            var hasWerewolf = table.Seats.Any(s => s.FinalCard.Role == Role.Werewolf);
            var hasMinion = table.Seats.Any(s => s.FinalCard.Role == Role.Minion);

            if (deaths.Count() > 0 && deaths.All(s => s.FinalCard.Role == Role.Tanner))
            {
                // 只死了皮匠
                return Camp.None;
            }
            else if (hasWerewolf)
            {
                // 有狼人，死任一狼人
                if (deaths.Any(s => s.FinalCard.Role == Role.Werewolf))
                {
                    // 村民赢
                    return Camp.Villiage;
                }
                else
                {
                    // 狼人赢
                    return Camp.Werewolf;
                }
            }
            else if (hasMinion)
            {
                // 没狼人 有爪牙
                if (deaths.Any(s => s.FinalCard.Role == Role.Minion) && !deaths.Any(s => s.FinalCard.Role != Role.Minion && s.FinalCard.Role != Role.Tanner))
                {
                    // 村民赢
                    return Camp.Villiage;
                }
                else if (!deaths.Any(s => s.FinalCard.Role == Role.Minion) && deaths.Any(s => s.FinalCard.Role != Role.Minion && s.FinalCard.Role != Role.Tanner))
                {
                    // 狼人赢
                    return Camp.Werewolf;
                }
                else
                {
                    // 没有阵营赢
                    return Camp.None;
                }
            }
            else if (deaths.Count() == 0)
            {
                return Camp.Villiage;
            }
            else
            {
                return Camp.None;
            }
        }
    }
}
