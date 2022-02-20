using System;
using System.Linq;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Common
{
    public class DefaultWinningCampDecisionRule : IWinningCampDecisionRule
    {
        public Camp Judge(Table table)
        {
            var deaths = table.GetSeats().Where(s => s.Dead);
            var hasWerewolf = table.GetSeats().Any(s => s.GetFinalCard().Role == Role.Werewolf);
            var hasMinion = table.GetSeats().Any(s => s.GetFinalCard().Role == Role.Minion);

            if (deaths.Count() > 0 && deaths.All(s => s.GetFinalCard().Role == Role.Tanner))
            {
                // 只死了皮匠
                return Camp.None;
            }
            else if (hasWerewolf)
            {
                // 有狼人，死任一狼人
                if (deaths.Any(s => s.GetFinalCard().Role == Role.Werewolf))
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
                if (deaths.Any(s => s.GetFinalCard().Role == Role.Minion) && !deaths.Any(s => s.GetFinalCard().Role != Role.Minion && s.GetFinalCard().Role != Role.Tanner))
                {
                    // 村民赢
                    return Camp.Villiage;
                }
                else if (!deaths.Any(s => s.GetFinalCard().Role == Role.Minion) && deaths.Any(s => s.GetFinalCard().Role != Role.Minion && s.GetFinalCard().Role != Role.Tanner))
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
