using System;
using System.Collections.Generic;
using System.Linq;

namespace OneNightWerewolf.Core
{
    public interface IActionHandler
    {
        public Action Action { get; }
        public void Handle(Table table, Seat seat, Choice choice);
    }
}
