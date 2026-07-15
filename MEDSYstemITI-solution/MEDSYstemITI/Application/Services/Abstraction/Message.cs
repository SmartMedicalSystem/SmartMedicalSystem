using System;
using System.Collections.Generic;

namespace Application.Services.Abstraction
{
    public class Message
    {
        public IEnumerable<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = to;
            Subject = subject;
            Content = content;
        }
    }
}
