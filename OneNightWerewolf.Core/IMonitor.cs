using System;
using System.Collections.Generic;
using System.Linq;
using static OneNightWerewolf.Core.IMonitor;

namespace OneNightWerewolf.Core
{
    public interface IMonitor
    {
        void Print(string message, Table table);

        void Clear();

        IList<Message> Display();

        public class Message
        {
            public DateTime Time { get; set; }

            public string Content { get; set; }

            public Phase Phase { get; set; }

            public int RoundOrder { get; set; }

            public string RoundName { get; set; }

            public int RoundIndex { get; set; }
        }
    }

    public class HtmlMonitor : IMonitor
    {
        private List<Message> Messages = new List<Message>();

        public void Clear()
        {
            Messages.Clear();
        }

        public void Print(string message, Table table)
        {
            Messages.Add(new Message()
            {
                Time = DateTime.Now,
                Content = message,
                Phase = table.Round.Phase,
                RoundOrder = table.Round.Order,
                RoundName = table.Round.Name,
                RoundIndex = table.RoundIndex
            });
        }

        public IList<Message> Display()
        {
            return Messages;
        }
    }
}
