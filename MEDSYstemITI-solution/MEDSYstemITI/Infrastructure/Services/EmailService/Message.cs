using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Application.Services.EmailService
{
    public class Message
    {
        public List<MailboxAddress> To {  get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        
        public Message(IEnumerable<String> to , string subject , string content) 
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x =>  MailboxAddress.Parse(x)));
            Subject = subject ;
            Content = content ;
            
        }
    }
}
