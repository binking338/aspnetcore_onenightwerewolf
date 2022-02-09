using System;
namespace OneNightWerewolf.Core
{
    public interface IWinningCampDecisionRule
    {
        Camp Judge(Table table);
    }
}
