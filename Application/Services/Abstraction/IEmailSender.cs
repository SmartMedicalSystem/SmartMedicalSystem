using Application.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Abstraction
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
