using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
