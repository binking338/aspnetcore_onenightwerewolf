using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Common
{

    public class StartAbility : IAbility
    {
        public string Name => "查看自己的角色";

        public IOption[] Options => new IOption[] { new StartOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is StartRound);
    }

    public class VoteAbility : IAbility
    {
        public string Name => "投票";

        public IOption[] Options => new IOption[] { new VoteOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is VoteRound);
    }

    public class TicketsAbility : IAbility
    {
        public string Name => "投票结果";

        public IOption[] Options => new IOption[] { new TicketsOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is TicketsRound);
    }

    public class JudgeAbility : IAbility
    {
        public string Name => "游戏结局";

        public IOption[] Options => new IOption[] { new JudgeOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is JudgeRound);
    }

    public class OverAbility : IAbility
    {
        public string Name => "游戏结束";

        public IOption[] Options => new IOption[] { new ReadyOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is OverRound);
    }
}
