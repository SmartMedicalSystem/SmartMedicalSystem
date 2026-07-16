using Application.Services.Abstraction;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Infrastructure.Services.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task SendEmailAsync(Application.DTOs.Email.Message message)
        {
            try
            {
                var emailMessage = CreateEmailMessage(message);
                await Send(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Email send Failed", ex);
            }
        }

        private MimeMessage CreateEmailMessage(Application.DTOs.Email.Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
            foreach (var to in message.To)
            {
                emailMessage.To.Add(MailboxAddress.Parse(to));
            }
            emailMessage.Subject = message.Subject;

            var emailContent = new BodyBuilder
            {
                HtmlBody = message.Content,
                TextBody = "Please view this email in HTML mode."
            };
            emailMessage.Body = emailContent.ToMessageBody();

            return emailMessage;
        }

        private async Task Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, 587, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);

                    await client.SendAsync(mailMessage);

                }
                catch (Exception ex)
                {
                    throw new Exception("Email send Failed", ex);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
    
