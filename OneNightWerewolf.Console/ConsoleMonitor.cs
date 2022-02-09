using System;
using System.Collections.Generic;
using OneNightWerewolf.Core;
using static OneNightWerewolf.Core.IMonitor;

namespace OneNightWerewolf.Console
{
    public class ConsoleMonitor : IMonitor
    {
        private string Name { get; set; }
        public ConsoleMonitor(string name)
        {
            Name = name;
        }

        public void Clear()
        {
            System.Console.WriteLine("(clear)");
        }

        public IList<Message> Display()
        {
            return null;
        }

        public void Print(string message)
        {
            System.Console.WriteLine($"{Name}: {message}");
        }
    }
}
