using System;
using OneNightWerewolf.Core;

namespace OneNightWerewolf.Insomniac
{
    public class InsomniacAbility : IAbility
    {
        public string Name => "";

        public IOption[] Options => new IOption[] { new InsomniacOption() };

        public Func<IRound, bool> TriggerCondition => (round => round is InsomniacRound);
    }
}
