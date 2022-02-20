using System;
using System.Collections.Generic;
using System.Linq;
using static OneNightWerewolf.Core.IMonitor;

namespace OneNightWerewolf.Core
{
    public interface IMonitor
    {
        void Print(Message message);

        void Clear();

        List<Message> Messages { get; }

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

    public class DefaultMonitor : IMonitor
    {

        public void Clear()
        {
            Messages.Clear();
        }

        public void Print(Message message)
        {
            Messages.Add(message);
        }

        public void Print(string content, Table table)
        {
            Print(DefaultMonitor.Of(content, table));
        }

        public List<Message> Messages { get; set; } = new List<Message>();

        public static Message Of(string content, Table table)
        {
            return new Message()
            {
                Time = DateTime.Now,
                Content = content,
                Phase = table.GetRound().Phase,
                RoundOrder = table.GetRound().Order,
                RoundName = table.GetRound().Name,
                RoundIndex = table.RoundIndex
            };
        }
    }
}
