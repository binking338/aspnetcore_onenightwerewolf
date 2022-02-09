using System;
using System.Collections.Generic;
using System.Linq;
using static OneNightWerewolf.Core.IMonitor;

namespace OneNightWerewolf.Core
{
    public interface IMonitor
    {
        void Print(string message);

        void Clear();

        IList<Message> Display();

        public class Message
        {
            public DateTime Time { get; set; }

            public string Content { get; set; }
        }
    }

    public class HtmlMonitor : IMonitor
    {
        private List<Message> Messages = new List<Message>();

        public void Clear()
        {
            Messages.Clear();
        }

        public void Print(string message)
        {
            Messages.Add(new Message() { Time = DateTime.Now, Content = message });
        }

        public IList<Message> Display()
        {
            return Messages;
        }
    }
}
