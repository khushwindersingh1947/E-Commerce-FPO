﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Cms;
using System.Net.Mail;

namespace ECommerce.Services
{
    public class EmailSender : IEmailSender
    {

        private string _fromAddress;
        private string _username;
        private string _password;

        public EmailSender(IConfiguration configuration) 
        {
            _fromAddress = configuration["SenderEmail"];
            _username = configuration["GoogleUserName"];
            _password = configuration["GooglePassword"];
        }


        public async Task SendEmailAsync(string toEmail, string subject, string email)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Jattana Nursury", _fromAddress));
            message.To.Add(new MailboxAddress("Customer", toEmail));
            message.Subject = subject;

            var body = new MimeKit.BodyBuilder
            {
                HtmlBody = email
            };

            message.Body = body.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(_username, _password);
            await client.SendAsync(message);

            client.Disconnect(true);

        }
    }
}
