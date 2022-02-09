using System;
namespace OneNightWerewolf.Core
{
    public interface IAbility
    {
        public string Name { get; }

        public IOption[] Options { get; }

        public Func<IRound, bool> TriggerCondition { get; }
    }

    public class NoneAbility : IAbility
    {
        public string Name => "无操作";

        public IOption[] Options => new IOption[] { new NoneOption() };

        public Func<IRound, bool> TriggerCondition => (round => true);
    }
}
